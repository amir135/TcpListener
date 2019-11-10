using TcpExample.UI.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StructureMap;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;

using System.Text;
using System.Net;
using System.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using WebApplication1;

using Microsoft.Extensions.Hosting;
using TcpExample.Domain.Common;
using Microsoft.IdentityModel.Tokens;
using TcpExample.UI.Hubs;

namespace TcpExample.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var validationParams = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidateAudience = true,
                ValidAudience = Configuration["Token:Audience"],
                ValidateIssuer = true,
                ValidIssuer = Configuration["Token:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Token:SigningKey"])),
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = true,
                ValidateLifetime = true
            };

            TokenConfigurationParameteres.Issuer = Configuration["Token:Issuer"];
            TokenConfigurationParameteres.SignKey = Configuration["Token:SigningKey"];
            services.AddAuthentication(options => {
                options.DefaultScheme = "Cookies";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;

                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = Configuration["Token:Issuer"],
                    ValidAudience = Configuration["Token:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:SigningKey"]))
                };

            }).AddCookie("Cookies", options => {
                options.Cookie.Name = "auth_cookie";
                options.Cookie.SameSite = SameSiteMode.None;
                options.LoginPath = new PathString("/Account/Login/");
            });

            services
    .AddControllersWithViews()
    .AddRazorRuntimeCompilation();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddRazorPages()
                    .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                            factory.Create(typeof(Common));
                    })
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder)
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddControllersAsServices();
            services.AddSignalR();


            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);


            var container = services.AddStructureMap(Configuration);

            return container.GetInstance<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                SupportedUICultures = new List<CultureInfo> { new CultureInfo("tr-TR") },
                DefaultRequestCulture = new RequestCulture("tr-TR")
            });

           
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<DataHub>("/DataHub");
            });

        }
    }
}
