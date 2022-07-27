using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace izolabella.Backend.Objects.Structures.Backend
{
    public class User
    {
        [JsonConstructor]
        public User(string Id, UserCredentials Credentials)
        {
            this.Id = Id;
            this.Credentials = Credentials;
        }

        [JsonProperty("Id")]
        public virtual string Id { get; }

        [JsonProperty("Secret")]
        public UserCredentials Credentials { get; }
    }
}
