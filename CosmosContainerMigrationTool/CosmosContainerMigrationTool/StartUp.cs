using CosmosContainerMigrationTool;
using CosmosContainerMigrationTool.Abstractions;
using CosmosContainerMigrationTool.Configurations;
using CosmosContainerMigrationTool.Entities;
using CosmosContainerMigrationTool.Implementations;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

[assembly: FunctionsStartup(typeof(StartUp))]

namespace CosmosContainerMigrationTool
{
    public class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<ICollectionService<Document>, DocumentCollectionMigrationService>();
            builder.Services.AddScoped<IFieldNormalizationStrategy<Document, NormalizedDocument>, DocumentNameNormalizationStrategy>();
            builder.Services.AddScoped<IMigrateDocumentsCollectionService, MigrateDocumentsCollectionService>();


            builder.Services.AddOptions<CosmosDbConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("CosmosDb").Bind(settings);
                });

            builder.Services.AddOptions<ParametersConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Parameters").Bind(settings);
                });
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }
    }
}

