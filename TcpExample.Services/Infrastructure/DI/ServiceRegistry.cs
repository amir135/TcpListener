
using Microsoft.EntityFrameworkCore;
using StructureMap;
using TcpExample.DataAccessCore;

namespace TcpExample.ServiceCore.Infrastructure.DI
{
    public class ServiceRegistry : Registry
    {
        public ServiceRegistry(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TcpExampleDBContext>();

          
                optionsBuilder.UseSqlServer(connectionString);
           
            For<ITcpExampleDBContext>().Use<TcpExampleDBContext>()
                      .Ctor<DbContextOptions<TcpExampleDBContext>>("options")
                                  .Is(optionsBuilder.Options)
                                  .SelectConstructor(() => new TcpExampleDBContext(null));

            


        }
    }
}
