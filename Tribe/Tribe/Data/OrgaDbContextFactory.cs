using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Tribe.Data
{
    public class OrgaDbContextFactory : IDesignTimeDbContextFactory<OrgaDbContext>
    {
        public OrgaDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("OrgaDbContextConnection")
                ?? throw new InvalidOperationException("Connection string 'OrgaDbContextConnection' not found.");

            var optionsBuilder = new DbContextOptionsBuilder<OrgaDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new OrgaDbContext(optionsBuilder.Options);
        }
    }
}
