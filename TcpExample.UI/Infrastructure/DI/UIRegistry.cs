using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using StructureMap;

namespace TcpExample.UI.DI
{
    public class UIRegistry : Registry
    {
        public UIRegistry(IConfiguration configuration)
        {
            For<IMapper>().Use<Mapper>();
            Policies.SetAllProperties(y => y.OfType<IMapper>());

            For<IConfiguration>().Use(x => configuration);
        }
    }
}
