using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.Objects.Structures.Backend
{
    public interface IUserAuthenticationModel
    {
        public Task<string?> GetSecretFromHeadersAsync(NameValueCollection Headers);

        public Task<User> CreateNewUserAsync(string Secret);

        public Task<User?> AuthenticateUserAsync(string Secret);

        public bool CreateUserIfAuthNull { get; }
    }
}