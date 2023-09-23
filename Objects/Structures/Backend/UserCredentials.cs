using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace izolabella.Backend.Objects.Structures.Backend
{
    public class UserCredentials
    {
        public UserCredentials(string Secret)
        {
            this.Secret = Secret;
        }

        [JsonProperty("Inner")]
        public string Secret { get; }
    }
}