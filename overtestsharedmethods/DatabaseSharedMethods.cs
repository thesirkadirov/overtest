using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using CharSet = Pomelo.EntityFrameworkCore.MySql.Storage.CharSet;

namespace Sirkadirov.Overtest.Libraries.Shared.Methods
{
    
    public static class DatabaseSharedMethods
    {
        
        public static DbContextOptionsBuilder GetDbContextOptions(this DbContextOptionsBuilder dbContextOptionsBuilder, IConfiguration configuration)
        {
            
            /*
             * Configure connection
             */
            
            var connectionString = configuration.GetValue<string>("database:connection_string");
            
            // To workaround https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/projects
            const string migrationsAssembly = "owebapp";
            
            switch (configuration.GetValue<string>("database:provider").ToUpper())
            {
                
                case "MARIADB":
                case "MYSQL":
                    dbContextOptionsBuilder.UseMySql(connectionString, builder =>
                    {
                        builder.CharSet(CharSet.Utf8);
                        builder.MigrationsAssembly(migrationsAssembly);
                    });
                    break;
                
                case "SQLSERVER":
                    dbContextOptionsBuilder.UseSqlServer(connectionString, builder =>
                    {
                        builder.MigrationsAssembly(migrationsAssembly);
                    });
                    break;
                
                default:
                    throw new Exception("Unknown database provider are being used!");
                
            }
            
            return dbContextOptionsBuilder;
            
        }
        
        public static OvertestDatabaseContext GetDbContext(this IConfiguration configuration, ILoggerProvider loggerProvider)
        {
            var databaseContext = new OvertestDatabaseContext(new DbContextOptionsBuilder().GetDbContextOptions(configuration).Options);
            
            /*
             * Configure logging
             */
            
            databaseContext.GetService<ILoggerFactory>().AddProvider(loggerProvider);

            return databaseContext;

        }
        
    }
    
}