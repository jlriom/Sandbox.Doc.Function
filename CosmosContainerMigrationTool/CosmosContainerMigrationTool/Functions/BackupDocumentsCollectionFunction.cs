using CosmosContainerMigrationTool.Abstractions;
using CosmosContainerMigrationTool.Configurations;
using CosmosContainerMigrationTool.Dtos;
using CosmosContainerMigrationTool.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CosmosContainerMigrationTool.Functions
{
    public class BackupDocumentsCollectionFunction
    {
        private readonly ILogger<DeleteDocumentsCollectionFunction> _logger;
        private readonly IMigrateDocumentsCollectionService _migrateDocumentsCollectionService;
        private readonly ParametersConfiguration _parametersConfiguration;

        public BackupDocumentsCollectionFunction(ILogger<DeleteDocumentsCollectionFunction> log,
            IMigrateDocumentsCollectionService migrateDocumentsCollectionService,
            IOptions<ParametersConfiguration> parametersConfiguration)
        {
            _logger = log;
            _migrateDocumentsCollectionService = migrateDocumentsCollectionService;
            _parametersConfiguration = parametersConfiguration.Value;
        }

        [FunctionName("BackupDocumentsCollection")]
        [OpenApiOperation(operationId: "BackupDocumentsCollection", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Target **Container Name**")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(FailureResult), Description = "Technical Error")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(FailureResult), Description = "Business Error")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            try
            {
                string collectionName = req.Query["name"].ToString().Trim().ToLower();

                if (string.IsNullOrEmpty(collectionName))
                {
                    throw new ArgumentNullException(nameof(collectionName));
                }

                if (collectionName == _parametersConfiguration.ContainerName.Trim().ToLower())
                {
                    throw new ApplicationException($"Target Collection can not be the original collection '{_parametersConfiguration.ContainerName}'");
                }


                await _migrateDocumentsCollectionService.BackUpAsync(collectionName);

                string responseMessage = $" Back up '{collectionName}' has been created successfully.";

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

