using System;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.Libraries.Shared.Methods;

namespace Sirkadirov.Overtest.WebApplication
{
    
    public class Startup
    {
        
        private IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {

            ConfigureDataStorageServices();
            
            services.AddControllersWithViews();
            
            services.AddRazorPages().AddRazorRuntimeCompilation();

            services.AddLocalization(options => options.ResourcesPath = "Localization");

            services.AddMvc(options =>
            {

                // Require user to be authenticated by default
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));

            }).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization();

            void ConfigureDataStorageServices()
            {
                try
                {
                    
                    // Add primary database context
                    services.AddDbContext<OvertestDatabaseContext>(options => options.GetDbContextOptions(Configuration));
                    
                    // Add primary identity services
                    services.AddIdentity<User, IdentityRole<Guid>>(options =>
                    {
                        
                        /*
                         * Additional security configuration
                         */
                        
                        options.Password.RequiredLength = 8;
                        options.User.RequireUniqueEmail = true;
                        
                        /*
                         * Disable account confirmation services
                         * due to custom services usage
                         */
                        
                        options.SignIn.RequireConfirmedAccount = false;
                        options.SignIn.RequireConfirmedEmail = false;
                        options.SignIn.RequireConfirmedPhoneNumber = false;
                        
                    }).AddEntityFrameworkStores<OvertestDatabaseContext>();
                
                }
                catch (Exception innerException)
                {
                    throw new ApplicationException(
                        "Data storage services configuration failed! Review your Overtest configuration files and check database provider availability!",
                        innerException
                    );
                }
            }
            
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/System/Error/{0}");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            SetUpLocalization();
            
            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapControllerRoute("default", "{controller=Welcome}/{action=Welcome}/{id?}");
                
                MapArea("Installation", "Installer", "Welcome");
                MapArea("Administration", "Home", "Index");
                MapArea("Competition", "Competitions", "List");
                MapArea("Social", "Home", "Index");
                
                void MapArea(string areaName, string defaultController, string defaultAction)
                {
                    endpoints.MapAreaControllerRoute(
                        name: areaName,
                        areaName: areaName,
                        pattern: $"{{area={areaName}}}/{{controller={defaultController}}}/{{action={defaultAction}}}/{{id?}}"
                    );
                }

            });

            void SetUpLocalization()
            {
                
                // Default culture (Glory to Ukraine!)
                var defaultCulture = new CultureInfo("uk-UA");
                
                // Supported cultures list (UI and formats)
                var supportedCultures = new[]
                {
                    defaultCulture
                    // Other cultures coming soon
                };
                
                app.UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture(defaultCulture),
                    SupportedCultures = supportedCultures,
                    SupportedUICultures = supportedCultures
                });
                
            }
            
        }
        
    }
    
}