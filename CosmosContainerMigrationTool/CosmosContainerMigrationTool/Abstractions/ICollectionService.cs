using CosmosContainerMigrationTool.Entities;
using System.Threading.Tasks;

namespace CosmosContainerMigrationTool.Abstractions
{
    public interface ICollectionService<TEntity>
        where TEntity : Entity
    {
        Task CreateCollectionAsync(string containerName, string partitionKey);
        Task CopyCollectionAsync<TNormalizedEntity>(string sourceContainerName, string destinationContainerName, string partitionKey,
            IFieldNormalizationStrategy<TEntity, TNormalizedEntity> fieldNormalizationStrategy)
                where TNormalizedEntity : Entity;
        Task ReplaceCollectionAsync<TNormalizedEntity>(string sourceContainerName, string destinationContainerName, string partitionKey,
            IFieldNormalizationStrategy<TEntity, TNormalizedEntity> fieldNormalizationStrategy)
                where TNormalizedEntity : Entity;
        Task DeleteCollectionAsync(string containerName);
    }
}
