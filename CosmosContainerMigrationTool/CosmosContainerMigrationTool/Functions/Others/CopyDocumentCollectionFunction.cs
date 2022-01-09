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
    public class CopyDocumentCollectionFunction
    {
        private readonly ILogger<CopyDocumentCollectionFunction> _logger;
        private readonly ICollectionService<Document> _documentCollectionService;
        private readonly IFieldNormalizationStrategy<Document, NormalizedDocument> _documentNameNormalizationStrategy;

        public CopyDocumentCollectionFunction(ILogger<CopyDocumentCollectionFunction> log,
            ICollectionService<Document> documentCollectionService,
            IFieldNormalizationStrategy<Document, NormalizedDocument> documentNameNormalizationStrategy)

        {
            _logger = log;
            _documentCollectionService = documentCollectionService;
            _documentNameNormalizationStrategy = documentNameNormalizationStrategy;
        }

        [FunctionName("CopyDocumentCollection")]
        [OpenApiOperation(operationId: "CopyDocumentCollection", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(CopyCollectionRequest))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(FailureResult), Description = "Technical Error")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(FailureResult), Description = "Business Error")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> CopyDocumentCollection(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] CopyCollectionRequest copyCollectionRequest)
        {
            try
            {
                await _documentCollectionService
                    .CopyCollectionAsync(copyCollectionRequest.SourceContainerName, copyCollectionRequest.DestinationContainerName, copyCollectionRequest.PartitionKeyName, _documentNameNormalizationStrategy);

                string responseMessage = $"Collection {copyCollectionRequest.SourceContainerName} content copied to {copyCollectionRequest.DestinationContainerName}.";

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

