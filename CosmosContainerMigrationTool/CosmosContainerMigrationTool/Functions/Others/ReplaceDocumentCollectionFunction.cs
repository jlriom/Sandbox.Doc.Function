using CosmosContainerMigrationTool.Abstractions;
using CosmosContainerMigrationTool.Dtos;
using CosmosContainerMigrationTool.Entities;
using CosmosContainerMigrationTool.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CosmosContainerMigrationTool.Functions.Others
{
    public class ReplaceDocumentCollectionFunction
    {
        private readonly ILogger<ReplaceDocumentCollectionFunction> _logger;
        private readonly ICollectionService<Document> _documentCollectionService;
        private readonly IFieldNormalizationStrategy<Document, NormalizedDocument> _documentNameNormalizationStrategy;

        public ReplaceDocumentCollectionFunction(ILogger<ReplaceDocumentCollectionFunction> log,
            ICollectionService<Document> documentCollectionService,
            IFieldNormalizationStrategy<Document, NormalizedDocument> documentNameNormalizationStrategy)

        {
            _logger = log;
            _documentCollectionService = documentCollectionService;
            _documentNameNormalizationStrategy = documentNameNormalizationStrategy;
        }

        [FunctionName("ReplaceDocumentCollection")]
        [OpenApiOperation(operationId: "ReplaceDocumentCollection", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(ReplaceCollectionRequest))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(FailureResult), Description = "Technical Error")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(FailureResult), Description = "Business Error")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> ReplaceDocumentCollection(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] ReplaceCollectionRequest replaceCollectionRequest)
        {
            try
            {
                await _documentCollectionService
                    .ReplaceCollectionAsync(replaceCollectionRequest.SourceContainerName, replaceCollectionRequest.DestinationContainerName,
                        replaceCollectionRequest.PartitionKeyName, _documentNameNormalizationStrategy);

                string responseMessage = $"Collection {replaceCollectionRequest.SourceContainerName} replaced by {replaceCollectionRequest.DestinationContainerName} successfully .";

                _logger.LogInformation(responseMessage);
                return new OkObjectResult(responseMessage);

            }
            catch (ApplicationException ex)
            {
                ObjectResult result = ex.ToResult(StatusCodes.Status400BadRequest);
                _logger.LogError($"{result}");
                return result;
            }
            catch (Exception ex)
            {
                ObjectResult result = ex.ToResult(StatusCodes.Status500InternalServerError);
                _logger.LogError($"{result}");
                return result;
            }
        }
    }
}

