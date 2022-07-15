using izolabella.Backend.Objects.Exceptions;
using izolabella.Backend.Objects.Exceptions.Bases;
using izolabella.Backend.Objects.Structures.Backend;
using izolabella.Backend.Objects.Structures.Controllers.Arguments;
using izolabella.Backend.Objects.Structures.Controllers.Bases;
using izolabella.Backend.Objects.Structures.Controllers.Results;
using izolabella.Backend.REST.Objects.ErrorMessages.Base;
using izolabella.Util.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        #region constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Prefix">https://example.com:443/</param>
        public IzolabellaServer(Uri[] Prefixes, Controller? Self = null, HttpMethod[]? MethodsSupported = null, Assembly?[]? AssembliesToLoadFrom = null)
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
        public IzolabellaServer(Uri[] Prefixes,
                                Controller? Self = null,
                                HttpMethod[]? MethodsSupported = null,
                                IUserAuthenticationModel? AuthenticationModel = null)
        {
            this.Methods = MethodsSupported ?? this.Methods;
            foreach (Uri Prefix in Prefixes)
            {
                this.HttpListener.Prefixes.Add(Prefix.ToString());
            }
            this.Prefixes = Prefixes;
            this.Self = Self;
            this.AuthenticationModel = AuthenticationModel;
        }

        #endregion

        #region server properties

        public HttpListener HttpListener { get; } = new()
        {
            IgnoreWriteExceptions = false
        };

        public Uri[] Prefixes { get; }

        public Controller? Self { get; }

        public HttpMethod[] Methods { get; } = new[]
        {
            HttpMethod.Get,
            HttpMethod.Post,
            HttpMethod.Put,
            HttpMethod.Patch,
            HttpMethod.Options
        };

        public IReadOnlyList<IzolabellaEndpoint> Controllers => Util.BaseImplementationUtil.GetItems<IzolabellaEndpoint>(this.AssembliesToLoadFrom);

        private Assembly?[]? assembliesToLoadFrom;

        public Assembly?[] AssembliesToLoadFrom { get => this.assembliesToLoadFrom ?? new Assembly?[] { Assembly.GetEntryAssembly(), Assembly.GetExecutingAssembly(), Assembly.GetCallingAssembly() }; set => this.assembliesToLoadFrom = value; }

        #endregion

        #region events

        public delegate Task OnControllerErrorHandler(Exception Ex, IzolabellaEndpoint ThrownBy);
        public event OnControllerErrorHandler? OnEndpointError;

        public delegate Task OnEndpointCalledHandler(IzolabellaEndpoint Endpoint);
        public event OnEndpointCalledHandler? EndpointCalled;

        public delegate Task OnEndpointNotFoundHandler();
        public event OnEndpointNotFoundHandler? EndpointNotFound;

        public delegate Task OnUserCreatedHandler(User User);
        public event OnUserCreatedHandler? UserCreated;

        public delegate Task OnUserAuthenticatedHandler(User User);
        public event OnUserAuthenticatedHandler? UserSuccessfullyAuthenticated;

        public delegate Task<User?> OnUserNullHandler(NameValueCollection Headers);
        public event OnUserNullHandler? UserNeedsAuthentication;

        public delegate Task OnServerStartedHandler();
        public event OnServerStartedHandler? ServerStarted;

        public delegate Task OnServerStoppedHandler();
        public event OnServerStoppedHandler? ServerStopped;

        public delegate Task OnServerFatalErrorHandler(Exception Ex);
        public event OnServerFatalErrorHandler? ServerFatalError;

        #endregion

        #region user defined properties for configuring server

        public IUserAuthenticationModel? AuthenticationModel { get; set; }

        #endregion

        #region request helpers

        private async Task<IzolabellaControllerArgument> GetArgumentsForRequestAsync(HttpListenerContext Context)
        {
            if(Context.Request.InputStream.CanRead)
            {
                using StreamReader ClientStreamReader = new(Context.Request.InputStream);
                string R = await ClientStreamReader.ReadToEndAsync();
                object? O = JsonConvert.DeserializeObject<object>(R);
                HttpMethod? Method = this.Methods.FirstOrDefault(M => M.Method.ToLower(CultureInfo.InvariantCulture) == Context.Request.HttpMethod.ToLower(CultureInfo.InvariantCulture));
                if (Method != null)
                {
                    User? U = await this.TryGetUserAsync(Context);
                    return new IzolabellaControllerArgument(this, U, R, O, Method, Context.Request.Url?.Segments.LastOrDefault() ?? String.Empty, Context.Request.Url);
                }
                else
                {
                    throw new MethodNotSupportedException(Context.Request.HttpMethod);
                }
            }
            else
            {
                throw new IncompatibleStreamException();
            }
        }

        private async Task<User?> TryGetUserAsync(HttpListenerContext Context)
        {
            if (this.AuthenticationModel == null)
            {
                return null;
            }
            string? SecretFromHeaders = await this.AuthenticationModel.GetSecretFromHeadersAsync(Context.Request.Headers);
            if(SecretFromHeaders == null)
            {
                return null;
            }
            User? Auth = await this.AuthenticationModel.AuthenticateUserAsync(SecretFromHeaders);
            if (Auth == null && this.AuthenticationModel.CreateUserIfAuthNull)
            {
                Auth = await this.AuthenticationModel.CreateNewUserAsync(SecretFromHeaders);
                this.UserCreated?.Invoke(Auth);
            }
            else if(Auth == null && !this.AuthenticationModel.CreateUserIfAuthNull)
            {
                Auth = await (this.UserNeedsAuthentication?.Invoke(Context.Request.Headers) ?? Task.FromResult<User?>(null));
            }
            else if (Auth != null)
            {
                this.UserSuccessfullyAuthenticated?.Invoke(Auth);
            }
            return Auth;
        }

        #endregion

        #region start & stop

        public Task StartListeningAsync()
        {
            this.HttpListener.Start();
            this.ServerStarted?.Invoke();
            this.Self?.Update($"Listening on: {string.Join(", ", this.Prefixes.Select(P => P.Host + " - port " + P.Port.ToString(CultureInfo.InvariantCulture)))}");
            this.Self?.Update($"{this.Controllers.Count} {(this.Controllers.Count == 1 ? "endpoint controller" : "endpoint controllers")} initialized: {String.Join(", ", this.Controllers.Select(C => "/" + C.Route))}");
            new Task(async () =>
            {
                try
                {
                    while (true)
                    {
                        HttpListenerContext Context = await this.HttpListener.GetContextAsync().ConfigureAwait(false);
                        Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                        string? RouteTo = Context.Request.RawUrl?.Split('/', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(0);
                        IzolabellaEndpoint? Controller = this.Controllers
                        .FirstOrDefault(C => C.Route.ToLower(CultureInfo.InvariantCulture) == RouteTo?.ToLower(CultureInfo.InvariantCulture));
                        if (Controller != null)
                        {
                            if (Context.Response.OutputStream.CanWrite)
                            {
                                try
                                {
                                    IzolabellaControllerArgument Args = await this.GetArgumentsForRequestAsync(Context);
                                    IzolabellaAPIControllerResult Result = await Controller.RunAsync(Args);
                                    this.EndpointCalled?.Invoke(Controller);
                                    using StreamWriter StreamWriter = new(Context.Response.OutputStream);
                                    if (Result.Entity == null && Result.Bytes != null)
                                    {
                                        StreamWriter.BaseStream.Write(Result.Bytes);
                                    }
                                    else
                                    {
                                        StreamWriter.Write(JsonConvert.SerializeObject(Result.Entity));
                                    }
                                }
                                catch (Exception Ex)
                                {
                                    await Controller.OnErrorAsync(Ex);
                                    this.Self?.Update(Ex.ToString());
                                    OnEndpointError?.Invoke(Ex, Controller);
                                }
                            }
                            else
                            {
                                this.Self?.Update("Stream not writeable.");
                            }
                        }
                        else
                        {
                            this.EndpointNotFound?.Invoke();
                            this.Self?.Update("No endpoint matching the given name sent was found.");
                        }
                        Context.Response.OutputStream.Close();
                        Context.Response.OutputStream.Dispose();
                    }
                }
                catch(Exception Ex)
                {
                    this.ServerFatalError?.Invoke(Ex);
                }
            }).Start();
            return Task.CompletedTask;
        }

        public Task StopListening()
        {
            this.HttpListener.Stop();
            this.ServerStopped?.Invoke();
            return Task.CompletedTask;
        }

        #endregion
    }
}
