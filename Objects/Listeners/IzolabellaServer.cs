using izolabella.Backend.REST.Objects.ErrorMessages.Base;
using izolabella.Backend.REST.Objects.Structures.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public IzolabellaServer(Uri Prefix)
        {
            this.HttpListener = new();
            this.HttpListener.Prefixes.Add(Prefix.ToString());
            this.HttpListener.IgnoreWriteExceptions = true;
            this.endpoints = izolabella.Util.BaseImplementationUtil.GetItems<IzolabellaAPIController>();
            this.Prefix = Prefix;
        }

        private readonly List<IzolabellaAPIController> endpoints;
        public IReadOnlyList<IzolabellaAPIController> Endpoints => this.endpoints;

        public HttpListener HttpListener { get; }

        public Uri Prefix { get; }

        public void AddEndpoint(IzolabellaAPIController Endpoint)
        {
            this.endpoints.Add(Endpoint);
        }

        public async Task StartListeningAsync()
        {
            this.HttpListener.Start();
            while (true)
            {
                HttpListenerContext Context = await this.HttpListener.GetContextAsync();
                string? RouteTo = Context.Request.RawUrl?.Split('/').ElementAtOrDefault(3);
                Task<IzolabellaAPIControllerResult>? Response = this.Endpoints.FirstOrDefault(C => C.Route.ToLower() == RouteTo?.ToLower())?.RunAsync();
                if(Response != null)
                {
                    IzolabellaAPIControllerResult Result = await Response;
                    if (Context.Response.OutputStream.CanWrite)
                    {
                        using StreamWriter StreamWriter = new(Context.Response.OutputStream);
                        StreamWriter.Write(JsonConvert.SerializeObject(Result.Entity));
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
