using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CharSet = Pomelo.EntityFrameworkCore.MySql.Storage.CharSet;

namespace Sirkadirov.Overtest.Libraries.Shared.Methods
{
    
    public static class Database
    {
        
        public static DbContextOptionsBuilder GetDbContextOptions(this DbContextOptionsBuilder dbContextOptionsBuilder, IConfiguration configuration)
        {
            
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
            
            return dbContextOptionsBuilder;
            
        }
        
    }
    
}