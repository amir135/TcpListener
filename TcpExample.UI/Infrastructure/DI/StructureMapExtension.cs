
using TcpExample.UI.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using TcpExample.ServiceCore.Infrastructure.DI;

namespace TcpExample.UI.Infrastructure
{
    public static class StructureMapExtension
    {
        public static IContainer AddStructureMap(this IServiceCollection services, IConfiguration configuration)
        {
            var container = new Container();
            
                container.Configure(config =>
                {
                    config.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.Assembly("TcpExample.Services");    
                    x.Assembly("TcpExample.DataAccess");
                    
                    x.WithDefaultConventions();
                });
                    
                config.AddRegistry<AutoMapperRegistry>();
                config.AddRegistry(new UIRegistry(configuration));
               
                var connectionString = configuration.GetConnectionString("TcpExampleDBContext");
                config.AddRegistry(new ServiceRegistry(connectionString));

                config.Populate(services);
            });


            var configText = container.WhatDoIHave();
            return container;
        }


    }
}
