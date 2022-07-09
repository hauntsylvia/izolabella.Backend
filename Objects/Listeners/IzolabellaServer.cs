using izolabella.Backend.Objects.Structures.Controllers.Arguments;
using izolabella.Backend.Objects.Structures.Controllers.Bases;
using izolabella.Backend.Objects.Structures.Controllers.Results;
using izolabella.Backend.REST.Objects.ErrorMessages.Base;
using izolabella.Util.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public IzolabellaServer(Uri Prefix, Controller? Self = null)
        {
            this.HttpListener = new();
            this.HttpListener.Prefixes.Add(Prefix.ToString());
            this.HttpListener.IgnoreWriteExceptions = true;
            this.controllers = Util.BaseImplementationUtil.GetItems<IzolabellaController>(Assembly.GetCallingAssembly());
            this.Prefix = Prefix;
            this.Self = Self;
        }

        public delegate Task OnErrorHandler(Exception Ex, IzolabellaController ThrownBy);
        public event OnErrorHandler? OnError;

        private readonly List<IzolabellaController> controllers;
        public IReadOnlyList<IzolabellaController> Controllers => this.controllers;

        public HttpListener HttpListener { get; }

        public Uri Prefix { get; }

        public Controller? Self { get; }

        public void AddEndpoint(IzolabellaController Endpoint)
        {
            this.controllers.Add(Endpoint);
        }

        private static async Task<IEnumerable<IzolabellaControllerArgument>> GetArgumentsForRequestAsync(HttpListenerContext Context)
        {
            List<IzolabellaControllerArgument> Args = new();
            if(Context.Request.InputStream.CanRead)
            {
                using StreamReader ClientStreamReader = new(Context.Request.InputStream);
                string R = await ClientStreamReader.ReadToEndAsync();
                object? O = JsonConvert.DeserializeObject<object>(R);
                Args.Add(new(R, O));
            }
            return Args;
        }

        public async Task StartListeningAsync()
        {
            this.HttpListener.Start();
            this.Self?.Update("Server started!");
            while (true)
            {
                HttpListenerContext Context = await this.HttpListener.GetContextAsync();
                string? RouteTo = Context.Request.RawUrl?.Split('/', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(0);
                IzolabellaController? Controller = this.Controllers.FirstOrDefault(C => C.Route.ToLower() == RouteTo?.ToLower());
                if(Controller != null)
                {
                    if (Context.Response.OutputStream.CanWrite)
                    {
                        IEnumerable<IzolabellaControllerArgument> Args = await GetArgumentsForRequestAsync(Context);
                        try
                        {
                            IzolabellaAPIControllerResult Result = await Controller.RunAsync(Args);
                            using StreamWriter StreamWriter = new(Context.Response.OutputStream);
                            StreamWriter.Write(JsonConvert.SerializeObject(Result.Entity));
                        }
                        catch(Exception Ex)
                        {
                            await Controller.OnErrorAsync(Ex);
                            this.OnError?.Invoke(Ex, Controller);
                        }
                    }
                }
                Context.Response.OutputStream.Dispose();
            }
        }

        public Task StopListening()
        {
            this.HttpListener.Stop();
            return Task.CompletedTask;
        }
    }
}
