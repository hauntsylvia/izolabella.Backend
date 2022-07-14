using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace izolabella.Backend.Objects.Structures.Backend
{
    public interface IUserCredentials
    {
        [JsonIgnore]
        internal string Secret { get; }
    }
}
