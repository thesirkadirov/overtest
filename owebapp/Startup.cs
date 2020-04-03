using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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

            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
                
                endpoints.MapRazorPages();
                
            });
            
        }
        
    }
    
}