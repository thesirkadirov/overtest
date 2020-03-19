using System;
using System.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;

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
                    services.AddDbContext<OvertestDatabaseContext>(options =>
                    {

                        /*
                         * Select database provider
                         */

                        switch (Configuration["application:database:provider"].ToUpper())
                        {

                            // Pomelo.EntityFrameworkCore.MySql
                            case "MYSQL":
                            case "MARIADB":
                                options.UseMySql(
                                    Configuration["server:database:connection_string"],
                                    builder => { builder.CharSet(CharSet.Utf8); }
                                );
                                break;

                            // Microsoft.EntityFrameworkCore.SqlServer
                            case "SQLSERVER":
                                options.UseSqlServer(Configuration["application:database:connection_string"]);
                                break;

                            default:
                                throw new DataException("Overtest database provider is unknown or not specified!");
                        }

                    });
                    
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