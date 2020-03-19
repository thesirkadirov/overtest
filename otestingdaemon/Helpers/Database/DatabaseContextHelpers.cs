using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Sirkadirov.Overtest.Libraries.Shared.Database;

namespace Sirkadirov.Overtest.TestingDaemon.Helpers.Database
{
    
    public static class DatabaseContextHelpers
    {
        
        public static DbContextOptions<OvertestDatabaseContext> GetDbContextOptions(IConfiguration configuration)
        {
                
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<OvertestDatabaseContext>();
            var connectionString = configuration.GetValue<string>("database:connection_string");

            switch (configuration.GetValue<string>("database:provider").ToUpper())
            {
                    
                case "MARIADB":
                case "MYSQL":
                    dbContextOptionsBuilder.UseMySql(connectionString, builder => builder.CharSet(CharSet.Utf8));
                    break;
                    
                case "SQLSERVER":
                    dbContextOptionsBuilder.UseSqlServer(connectionString);
                    break;
                    
                default:
                    throw new Exception("Unknown database provider are being used!");
                    
            }

            return dbContextOptionsBuilder.Options;

        }
        
    }
    
}