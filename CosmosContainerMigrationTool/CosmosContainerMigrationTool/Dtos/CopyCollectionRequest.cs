namespace CosmosContainerMigrationTool.Dtos
{
    public class CopyCollectionRequest
    {
        public string SourceContainerName { get; set; }
        public string DestinationContainerName { get; set; }
        public string PartitionKeyName { get; set; }

    }
}
