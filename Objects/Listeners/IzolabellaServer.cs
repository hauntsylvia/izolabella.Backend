using izolabella.Backend.Objects.Exceptions;
using izolabella.Backend.Objects.Exceptions.Bases;
using izolabella.Backend.Objects.Structures.Controllers.Arguments;
using izolabella.Backend.Objects.Structures.Controllers.Bases;
using izolabella.Backend.Objects.Structures.Controllers.Results;
using izolabella.Backend.REST.Objects.ErrorMessages.Base;
using izolabella.Util.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.REST.Objects.Listeners
{
    public class IzolabellaServer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Prefix">https://example.com:443/</param>
        public IzolabellaServer(Uri[] Prefixes, Controller? Self = null, HttpMethod[]? MethodsSupported = null, Assembly[]? AssembliesToLoadFrom = null)
        {
            this.Methods = MethodsSupported ?? this.Methods;
            this.assembliesToLoadFrom = AssembliesToLoadFrom;
            foreach (Uri Prefix in Prefixes)
            {
                this.HttpListener.Prefixes.Add(Prefix.ToString());
            }
            this.Prefixes = Prefixes;
            this.Self = Self;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Prefix">https://example.com:443/</param>
        public IzolabellaServer(Uri[] Prefixes, Controller? Self = null)
        {
            foreach(Uri Prefix in Prefixes)
            {
                this.HttpListener.Prefixes.Add(Prefix.ToString());
            }
            this.Prefixes = Prefixes;
            this.Self = Self;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Prefix">https://example.com:443/</param>
        public IzolabellaServer(Uri[] Prefixes, Controller? Self = null, HttpMethod[]? MethodsSupported = null, Func<IzolabellaServerException, object?>? OnServerError = null)
        {
            this.Methods = MethodsSupported ?? this.Methods;
            foreach (Uri Prefix in Prefixes)
            {
                this.HttpListener.Prefixes.Add(Prefix.ToString());
            }
            this.Prefixes = Prefixes;
            this.Self = Self;
            this.OnServerError = OnServerError;
        }

        public HttpMethod[] Methods { get; } = new[]
        {
            HttpMethod.Get,
            HttpMethod.Post,
            HttpMethod.Put,
            HttpMethod.Patch
        };

        private Assembly[]? assembliesToLoadFrom;

        public Assembly[] AssembliesToLoadFrom { get => this.assembliesToLoadFrom ?? new Assembly[] { Assembly.GetCallingAssembly() }; set => this.assembliesToLoadFrom = value; }

        public delegate Task OnControllerErrorHandler(Exception Ex, IzolabellaController ThrownBy);
        public event OnControllerErrorHandler? OnControllerError;

        public Func<IzolabellaServerException, object?>? OnServerError { get; }

        public IReadOnlyList<IzolabellaController> Controllers => Util.BaseImplementationUtil.GetItems<IzolabellaController>(this.AssembliesToLoadFrom);

        public HttpListener HttpListener { get; } = new()
        {
            IgnoreWriteExceptions = true
        };

        public Uri[] Prefixes { get; }

        public Controller? Self { get; }

        private async Task<IzolabellaControllerArgument> GetArgumentsForRequestAsync(HttpListenerContext Context)
        {
            if(Context.Request.InputStream.CanRead)
            {
                using StreamReader ClientStreamReader = new(Context.Request.InputStream);
                string R = await ClientStreamReader.ReadToEndAsync();
                object? O = JsonConvert.DeserializeObject<object>(R);
                HttpMethod? Method = this.Methods.FirstOrDefault(M => M.Method.ToLower(CultureInfo.InvariantCulture) == Context.Request.HttpMethod.ToLower(CultureInfo.InvariantCulture));
                return Method != null
                    ? (new(this, R, O, Method))
                    : throw new MethodNotSupportedException(Context.Request.HttpMethod);
            }
            else
            {
                throw new IncompatibleStreamException();
            }
        }

        public Task StartListeningAsync()
        {
            this.HttpListener.Start();
            this.Self?.Update($"{this.Controllers.Count} {(this.Controllers.Count == 1 ? "endpoint controller" : "endpoint controllers")} initialized.");
            new Thread(async () =>
            {
                while (true)
                {
                    HttpListenerContext Context = await this.HttpListener.GetContextAsync();
                    string? RouteTo = Context.Request.RawUrl?.Split('/', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(0);
                    IzolabellaController? Controller = this.Controllers.FirstOrDefault(C => C.Route.ToLower(CultureInfo.InvariantCulture) == RouteTo?.ToLower(CultureInfo.InvariantCulture));
                    if (Controller != null)
                    {
                        if (Context.Response.OutputStream.CanWrite)
                        {
                            try
                            {
                                IzolabellaControllerArgument Args = await this.GetArgumentsForRequestAsync(Context);
                                IzolabellaAPIControllerResult Result = await Controller.RunAsync(Args);
                                using StreamWriter StreamWriter = new(Context.Response.OutputStream);
                                StreamWriter.Write(JsonConvert.SerializeObject(Result.Entity));
                            }
                            catch (IzolabellaServerException Ex)
                            {
                                object Return = this.OnServerError?.Invoke(Ex) ?? Ex.Message;
                            }
                            catch (Exception Ex)
                            {
                                await Controller.OnErrorAsync(Ex);
                                OnControllerError?.Invoke(Ex, Controller);
                            }
                        }
                    }
                    Context.Response.OutputStream.Dispose();
                }
            }).Start();
            return Task.CompletedTask;
        }

        public Task StopListening()
        {
            this.HttpListener.Stop();
            return Task.CompletedTask;
        }
    }
}
