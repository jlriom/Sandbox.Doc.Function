using CosmosContainerMigrationTool.Abstractions;
using CosmosContainerMigrationTool.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosContainerMigrationTool.Implementations
{
    public abstract class CollectionService<TEntity> : ICollectionService<TEntity>
        where TEntity : Entity
    {

        protected readonly ILogger<CollectionService<TEntity>> Logger;
        protected readonly Database Database;

        public CollectionService(ILogger<CollectionService<TEntity>> logger, string stringConnection, string databaseName)
        {
            Logger = logger;
            CosmosClient cosmosClient = new CosmosClient(stringConnection, new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
                AllowBulkExecution = true
            });
            Database = cosmosClient.GetDatabase(databaseName);
        }

        public virtual async Task CopyCollectionAsync<TNormalizedEntity>(string sourceContainerName, string destinationContainerName, string partitionKey,
            IFieldNormalizationStrategy<TEntity, TNormalizedEntity> fieldNormalizationStrategy)
            where TNormalizedEntity : Entity
        {

            IEnumerable<TEntity> documentsSourceList = await ReadContainerContentAsync(sourceContainerName);

            IEnumerable<TNormalizedEntity> documentsDestinationList = ApplyNormalization(documentsSourceList, fieldNormalizationStrategy);

            await AddContentToDestination(destinationContainerName, documentsDestinationList);

        }


        public virtual async Task CreateCollectionAsync(string containerName, string partitionKey)
        {
            string partitionKeyPath = partitionKey.StartsWith("/") ? partitionKey : $"/{partitionKey}";

            ContainerProperties containerDef = new ContainerProperties
            {
                Id = containerName,
                PartitionKeyPath = partitionKeyPath
            };

            await Database.CreateContainerAsync(containerDef);
        }

        public virtual async Task DeleteCollectionAsync(string containerName)
        {
            Container container = Database.GetContainer(containerName);
            await container.DeleteContainerAsync();
        }

        public virtual async Task ReplaceCollectionAsync<TNormalizedEntity>(string sourceContainerName, string destinationContainerName, string partitionKey,
            IFieldNormalizationStrategy<TEntity, TNormalizedEntity> fieldNormalizationStrategy)
            where TNormalizedEntity : Entity
        {
            await DeleteCollectionAsync(destinationContainerName);
            await CreateCollectionAsync(destinationContainerName, partitionKey);
            await CopyCollectionAsync(sourceContainerName, destinationContainerName, partitionKey, fieldNormalizationStrategy);
        }

        protected virtual async Task<IEnumerable<TEntity>> ReadContainerContentAsync(string sourceContainerName)
        {
            List<TEntity> entities = new List<TEntity>();

            Container container = Database.GetContainer(sourceContainerName);

            FeedIterator<TEntity> iterator = container.GetItemQueryIterator<TEntity>("select * from c");

            while (iterator.HasMoreResults)
            {
                FeedResponse<TEntity> entitiesPage = await iterator.ReadNextAsync();

                entities.AddRange(entitiesPage);
            }

            return entities;
        }

        protected virtual IEnumerable<TNormalizedEntity> ApplyNormalization<TNormalizedEntity>(IEnumerable<TEntity> sourceList, IFieldNormalizationStrategy<TEntity, TNormalizedEntity> fieldNormalizationStrategy)
            where TNormalizedEntity : Entity
        {
            List<TNormalizedEntity> entities = new List<TNormalizedEntity>();

            foreach (TEntity entity in sourceList)
            {
                TNormalizedEntity normalizedEntity = fieldNormalizationStrategy.NormalizeField(entity);
                entities.Add(normalizedEntity);
            }

            return entities;
        }

        protected virtual async Task AddContentToDestination<TargetEntity>(string destinationContainerName, IEnumerable<TargetEntity> destinationList)
            where TargetEntity : Entity
        {
            Container container = Database.GetContainer(destinationContainerName);


            int count = destinationList.ToList().Count;

            Logger.LogInformation($"Adding {count} entities into collection {destinationContainerName}....");

            // ------ one by one version

            int numEntitiesAdded = 0;

            foreach (TargetEntity entity in destinationList)
            {
                await container.CreateItemAsync(entity, new PartitionKey(entity.Name));
                numEntitiesAdded++;

                if (numEntitiesAdded % 100 == 0)
                {
                    Logger.LogInformation($"{numEntitiesAdded} / {count} entities added....");
                }
            }

            Logger.LogInformation($"{numEntitiesAdded} / {count} entities added. End");

            // ------ Bulk copy version

            //var entitiesCount = destinationList.ToList().Count;

            //BulkOperations<TargetEntity> bulkOperations = new BulkOperations<TargetEntity>(entitiesCount);
            //foreach (var entity in destinationList)
            //{
            //    bulkOperations.Tasks.Add(OperationHandler.CaptureOperationResponse(container.CreateItemAsync(entity, new PartitionKey(entity.Name)), entity));

            //}
            //await Task.CompletedTask;
        }
    }
}
