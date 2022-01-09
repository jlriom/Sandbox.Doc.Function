using CosmosContainerMigrationTool.Configurations;
using CosmosContainerMigrationTool.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CosmosContainerMigrationTool.Implementations
{
    public class DocumentCollectionMigrationService : CollectionService<Document>
    {
        public DocumentCollectionMigrationService(ILogger<DocumentCollectionMigrationService> logger, IConfiguration configuration, IOptions<CosmosDbConfiguration> cosmosDbConfiguration)
            : base(logger, configuration.GetConnectionString("Default"), cosmosDbConfiguration.Value.DatabaseName)
        {
        }
    }
}
