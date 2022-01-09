using Newtonsoft.Json;
using System;

namespace CosmosContainerMigrationTool.Entities
{
    public abstract class Entity
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
