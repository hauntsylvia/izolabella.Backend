using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.Objects.Structures.Backend
{
    public interface IUserAuthenticationModel<TUser> where TUser : User
    {
        public Task<TUser> CreateNewUserAsync(NameValueCollection Headers);

        public Task<TUser?> AuthenticateUserAsync(NameValueCollection Headers);

        public bool CreateUserIfAuthNull { get; }
    }
}
