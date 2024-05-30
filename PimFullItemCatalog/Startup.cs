using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PimItemFullCatalog.Interfaces;
using PimItemFullCatalog.Services;
using AzureFunctions.Extensions.Swashbuckle;
using Azure.Storage.Blobs;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;


[assembly: FunctionsStartup(typeof(PimItemFullCatalog.Functions.Startup))]
namespace PimItemFullCatalog.Functions
{
    public class Startup : FunctionsStartup
    {
        const string _connString = "DefaultEndpointsProtocol=https;AccountName=steslteamcloudshell;AccountKey={THE_ACCOUNT_KEY_FROM_AZURE};EndpointSuffix=core.windows.net";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly(), opts =>
            {
                opts.AddCodeParameter = true;
                opts.Documents = new[]
                {
                    new SwaggerDocument
                    {
                        Name = "v1",
                        Title = "Swagger Document",
                        Description = "Swagger UI for Azure Functions",
                        Version = "v1"
                    }
                };
                opts.ConfigureSwaggerGen = x =>
                {
                    x.CustomOperationIds(apiDesc =>
                    {
                        return apiDesc.TryGetMethodInfo(out MethodInfo mInfo) ? mInfo.Name : default(Guid).ToString();
                    });
                };
            });

            var services = builder.Services;

            services.AddSingleton<IBlobService, BlobService>();
            services.AddSingleton(_ => new BlobServiceClient(_connString));
        
        }
    }
}
