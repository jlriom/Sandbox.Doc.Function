using System.Threading.Tasks;

namespace CosmosContainerMigrationTool.Abstractions
{
    public interface IMigrateDocumentsCollectionService
    {
        Task MigrateAsync();
        Task BackUpAsync(string newContainerName);
        Task DeleteAsync(string containerName);
    }
}
