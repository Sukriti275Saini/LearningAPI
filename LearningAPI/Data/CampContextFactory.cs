using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LearningAPI.Data
{
    public class CampContextFactory : IDesignTimeDbContextFactory<CampContext>
    {
        public CampContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json")
              .Build();

            return new CampContext(new DbContextOptionsBuilder<CampContext>().Options, config);
        }
    }
}
