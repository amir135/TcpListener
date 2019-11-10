using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace TcpExample.Domain.Common
{
    public class CurrentUser
    {
        public static UserClaimsInfo GetUser(ClaimsPrincipal User)
        {
            return new UserClaimsInfo()
            {
                Fullname = User?.FindFirst(ClaimTypes.Name).Value,
                UserId = User?.FindFirst("UserId").Value
            };
        }
    }
}
