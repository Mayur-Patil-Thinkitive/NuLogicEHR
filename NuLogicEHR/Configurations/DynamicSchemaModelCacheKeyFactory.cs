using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NuLogicEHR.Configurations
{
    public class DynamicSchemaModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
        {
            return Create(context, false);
        }

        public object Create(DbContext context, bool designTime)
        {
            if (context is ApplicationDbContext appContext)
            {
                return (context.GetType(), appContext.Schema, designTime);
            }
            return (context.GetType(), designTime);
        }
    }
}
