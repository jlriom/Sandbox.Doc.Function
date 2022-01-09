using CosmosContainerMigrationTool.Abstractions;
using CosmosContainerMigrationTool.Configurations;
using CosmosContainerMigrationTool.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace CosmosContainerMigrationTool.Implementations
{
    public class MigrateDocumentsCollectionService : IMigrateDocumentsCollectionService
    {

        private readonly ICollectionService<Document> _documentCollectionService;
        private readonly ParametersConfiguration _parametersConfiguration;
        private readonly IFieldNormalizationStrategy<Document, NormalizedDocument> _documentNameNormalizationStrategy;

        public MigrateDocumentsCollectionService(
            ICollectionService<Document> documentCollectionService,
            IFieldNormalizationStrategy<Document, NormalizedDocument> documentNameNormalizationStrategy,
            IOptions<ParametersConfiguration> parametersConfiguration)
        {
            _documentCollectionService = documentCollectionService;
            _documentNameNormalizationStrategy = documentNameNormalizationStrategy;
            _parametersConfiguration = parametersConfiguration.Value;
        }

        public async Task BackUpAsync(string newContainerName)
        {
            await _documentCollectionService.CreateCollectionAsync(newContainerName, _parametersConfiguration.PartitionKey);

            await _documentCollectionService.CopyCollectionAsync(_parametersConfiguration.ContainerName, newContainerName, _parametersConfiguration.PartitionKey, new VoidNormalizationStrategy<Document, Document>());
        }

        public async Task DeleteAsync(string containerName)
        {
            await _documentCollectionService.DeleteCollectionAsync(containerName);
        }

        public async Task MigrateAsync()
        {
            string workingContainerName = $"{_parametersConfiguration.ContainerName}_bak_{DateTime.UtcNow.ToString("yyyyMMdd_hhmmss")}";

            await _documentCollectionService.CreateCollectionAsync(workingContainerName, _parametersConfiguration.PartitionKey);

            await _documentCollectionService.CopyCollectionAsync(_parametersConfiguration.ContainerName, workingContainerName, _parametersConfiguration.PartitionKey, _documentNameNormalizationStrategy);

            await _documentCollectionService.ReplaceCollectionAsync(workingContainerName, _parametersConfiguration.ContainerName, _parametersConfiguration.PartitionKey, _documentNameNormalizationStrategy);

            await _documentCollectionService.DeleteCollectionAsync(workingContainerName);
        }
    }
}
