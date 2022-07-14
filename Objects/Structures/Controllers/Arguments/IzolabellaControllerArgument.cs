using izolabella.Backend.Objects.Structures.Backend;
using izolabella.Backend.Objects.Structures.Controllers.Bases;
using izolabella.Backend.REST.Objects.Listeners;
using izolabella.Storage.Objects.Structures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.Objects.Structures.Controllers.Arguments
{
    public class IzolabellaControllerArgument
    {
        public IzolabellaControllerArgument(IzolabellaServer Server, User? User, string? DefaultBody, object? Entity, HttpMethod Method, string WasInUri, Uri? UriSent)
        {
            this.Server = Server;
            this.User = User;
            this.DefaultBody = DefaultBody;
            this.Entity = Entity;
            this.Method = Method;
            this.WasInUri = WasInUri;
            this.UriSent = UriSent;
        }

        public IzolabellaServer Server { get; }

        public User? User { get; }

        private string? DefaultBody { get; }

        public object? Entity { get; }

        public HttpMethod Method { get; }

        public string WasInUri { get; }

        public Uri? UriSent { get; }

        public Task<bool> TryParseAsync<T>(out T? Result)
        {
            Result = JsonConvert.DeserializeObject<T>(this.DefaultBody ?? string.Empty);
            return Task.FromResult(Result != null);
        }

        public bool TryParse<T>(out T? Result)
        {
            try
            {
                Result = this.DefaultBody != null && this.DefaultBody.Length > 0 ? JsonConvert.DeserializeObject<T>(this.DefaultBody) : default;
            }
            catch
            {
                Result = default;
            }
            return Result != null;
        }
    }
}
