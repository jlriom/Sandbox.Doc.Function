namespace CosmosContainerMigrationTool.Dtos
{
    public class ReplaceCollectionRequest
    {
        public string SourceContainerName { get; set; }
        public string DestinationContainerName { get; set; }
        public string PartitionKeyName { get; set; }

    }
}
