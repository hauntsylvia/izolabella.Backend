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
        public User(string DisplayAlias, string Id, UserCredentials Credentials)
        {
            this.DisplayAlias = DisplayAlias;
            this.Id = Id;
            this.Credentials = Credentials;
        }

        [JsonProperty("DisplayName")]
        public string DisplayAlias { get; set; }

        [JsonProperty("Id")]
        public string Id { get; }

        [JsonProperty("Secret")]
        public UserCredentials Credentials { get; }
    }
}
