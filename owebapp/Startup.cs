using System;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
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
            
            ConfigureAuthServices();
            
            services.AddMvc(options =>
            {

                // Require user to be authenticated by default
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));

            }).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization();

            services.AddResponseCompression(options => options.EnableForHttps = true);
            
            void ConfigureDataStorageServices()
            {
                try
                {
                    
                    // Add primary database context
                    services.AddDbContext<OvertestDatabaseContext>();
                    
                    // Add primary identity services
                    services.AddIdentity<User, IdentityRole<Guid>>(options =>
                    {
                        
                        /*
                         * Additional security configuration
                         */
                        
                        options.User.RequireUniqueEmail = true;
                        options.Password.RequireDigit = true;
                        options.Password.RequiredLength = 8;
                        options.Password.RequireLowercase = true;
                        options.Password.RequireUppercase = true;
                        options.Password.RequiredUniqueChars = 3;
                        options.Password.RequireNonAlphanumeric = true;
                        
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

            void ConfigureAuthServices()
            {

                services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Auth/Authorization";
                    options.LogoutPath = "/Auth/LogOut";
                    options.AccessDeniedPath = "/Security/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.Cookie.Name = "OvertestAuthCookie";
                });
            }
            
        }
        
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, OvertestDatabaseContext databaseContext)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Security/Error/{0}");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            SetUpLocalization();

            app.UseResponseCompression();
            
            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Welcome}/{action=Welcome}/{id?}"
                );

                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

            });

            databaseContext.Database.Migrate();

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