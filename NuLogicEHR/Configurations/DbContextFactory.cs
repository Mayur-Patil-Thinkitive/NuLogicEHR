using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NuLogicEHR.Configurations
{
    public class DbContextFactory
    {
        private readonly IConfiguration _configuration;

        public DbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ApplicationDbContext CreateContext(string schema)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString)
                .ReplaceService<IModelCacheKeyFactory, DynamicSchemaModelCacheKeyFactory>()
                .Options;

            return new ApplicationDbContext(options, schema);
        }
    }
}
