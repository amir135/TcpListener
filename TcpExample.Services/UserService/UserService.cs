using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpExample.DataAccessCore;
using TcpExample.Domain.DBModel;

namespace TcpExample.Domain.Common.UserService
{
    public class UserService : IUserService
    {
        ITcpExampleDBContext db;
        public UserService(ITcpExampleDBContext dbContext)
        {
            db = dbContext;
        }
        public Result<int> getUserCount()
        {
            Result<int> result = new Result<int>();
            try
            {
                var count = db.Users.Where(a => !a.IsDeleted).Count();

                result.Success = true;
                result.Data = count;
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
            }
            return result;
        }
        public Result<User> GetUseer(Guid UserId)
        {
            Result<User> result = new Result<User>();
            try
            {

                result.Data = db.Users.FirstOrDefault(a => a.Id == UserId);
                if (result.Data == null)
                {
                    result.Success = false;
                    result.Message = "User is not Exists !!"; ;
                }
                else
                {
                    result.Success = true;
                }
            }

            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public Result<User> GetUseerByUserNamePassword(string UserName, string Password)
        {
            Result<User> result = new Result<User>();
            try {
                var password = HashConvertor.GetHashString(Password);
                var username = UserName.ToLower();

                result.Data=db.Users.FirstOrDefault(a => a.UserName == username && a.Password == password);
                if (result.Data == null)
                {
                    result.Success = false;
                    result.Message = "Username or password is not correct !!"; ;
                }
                else {
                    result.Success = true;
                }
            }

            catch (Exception ex) {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
