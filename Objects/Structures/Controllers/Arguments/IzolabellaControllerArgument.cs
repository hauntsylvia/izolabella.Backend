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
        public IzolabellaControllerArgument(string? DefaultBody, object? Entity, HttpMethod Method)
        {
            this.DefaultBody = DefaultBody;
            this.Entity = Entity;
            this.Method = Method;
        }

        private string? DefaultBody { get; }

        public object? Entity { get; }

        public HttpMethod Method { get; }

        public Task<bool> TryParseAsync<T>(out T? Result)
        {
            Result = JsonConvert.DeserializeObject<T>(DefaultBody ?? string.Empty);
            return Task.FromResult(Result != null);
        }

        public bool TryParse<T>(out T? Result)
        {
            try
            {
                Result = DefaultBody != null && DefaultBody.Length > 0 ? JsonConvert.DeserializeObject<T>(DefaultBody) : default;
            }
            catch
            {
                Result = default;
            }
            return Result != null;
        }
    }
}
