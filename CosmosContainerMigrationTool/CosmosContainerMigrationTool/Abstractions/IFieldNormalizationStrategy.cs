using CosmosContainerMigrationTool.Entities;

namespace CosmosContainerMigrationTool.Abstractions
{
    public interface IFieldNormalizationStrategy<TEntity, TNormalizedEntity>
        where TEntity : Entity
        where TNormalizedEntity : Entity
    {
        TNormalizedEntity NormalizeField(TEntity entity);
    }
}
