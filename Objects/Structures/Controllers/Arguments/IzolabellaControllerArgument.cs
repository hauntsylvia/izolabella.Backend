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
        public IzolabellaControllerArgument(string? DefaultBody, object? Entity)
        {
            this.DefaultBody = DefaultBody;
            this.Entity = Entity;
        }

        private string? DefaultBody { get; }

        public object? Entity { get; }

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
