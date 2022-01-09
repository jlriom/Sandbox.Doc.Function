using CosmosContainerMigrationTool.Abstractions;
using CosmosContainerMigrationTool.Dtos;
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

namespace CosmosContainerMigrationTool.Functions
{
    public class MigrateDocumentsFunction
    {
        private readonly ILogger<MigrateDocumentsFunction> _logger;
        private readonly IMigrateDocumentsCollectionService _migrateDocumentsCollectionService;
        public MigrateDocumentsFunction(ILogger<MigrateDocumentsFunction> log,
            IMigrateDocumentsCollectionService migrateDocumentsCollectionService)
        {
            _logger = log;
            _migrateDocumentsCollectionService = migrateDocumentsCollectionService;
        }

        [FunctionName("MigrateDocuments")]
        [OpenApiOperation(operationId: "MigrateDocuments", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(FailureResult), Description = "Technical Error")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(FailureResult), Description = "Business Error")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            try
            {
                await _migrateDocumentsCollectionService.MigrateAsync();
                string responseMessage = $"Document Collection has been migrated successfully.";
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

