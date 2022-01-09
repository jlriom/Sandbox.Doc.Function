using CosmosContainerMigrationTool.Abstractions;
using CosmosContainerMigrationTool.Entities;

namespace CosmosContainerMigrationTool.Implementations
{
    public class VoidNormalizationStrategy<TEntity, TNormalizedEntity> : IFieldNormalizationStrategy<TEntity, TNormalizedEntity>
        where TEntity : Entity
        where TNormalizedEntity : Entity
    {
        public TNormalizedEntity NormalizeField(TEntity entity)
        {
            return entity as TNormalizedEntity;
        }
    }
}
