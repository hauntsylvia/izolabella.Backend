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
        public User(string DisplayAlias, string Id, IUserCredentials Credentials)
        {
            this.DisplayAlias = DisplayAlias;
            this.Id = Id;
            this.Credentials = Credentials;
        }

        [JsonProperty("DisplayName")]
        public string DisplayAlias { get; }

        [JsonProperty("Id")]
        public string Id { get; }

        [JsonIgnore]
        public IUserCredentials Credentials { get; }
    }
}
