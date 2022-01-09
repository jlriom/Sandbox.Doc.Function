using CosmosContainerMigrationTool.Abstractions;
using CosmosContainerMigrationTool.Entities;
using CosmosContainerMigrationTool.Implementations.Normalization;

namespace CosmosContainerMigrationTool.Implementations
{
    public class DocumentNameNormalizationStrategy : IFieldNormalizationStrategy<Document, NormalizedDocument>
    {
        public NormalizedDocument NormalizeField(Document entity)
        {
            return new NormalizedDocument
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                NormalizedName = entity.Name.RemoveSpecialCharacters(SpecialCharacters.ReplacementChars).ToLower()
            };
        }
    }
}
