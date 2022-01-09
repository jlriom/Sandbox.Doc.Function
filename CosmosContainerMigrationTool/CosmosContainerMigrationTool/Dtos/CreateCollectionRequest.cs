namespace CosmosContainerMigrationTool.Dtos
{
    public class CreateCollectionRequest
    {
        public string ContainerName { get; set; }
        public string PartitionKeyName { get; set; }
    }
}
