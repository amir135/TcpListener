using System;
using System.Collections.Generic;
using System.Text;
using TcpExample.Domain.DBModel;

namespace TcpExample.Domain.Common.UserService
{
    public interface IUserService
    {
        Result<User> GetUseerByUserNamePassword(string UserName, string Password);
        Result<User> GetUseer(Guid UserId);
        Result<int> getUserCount();
    }
}
