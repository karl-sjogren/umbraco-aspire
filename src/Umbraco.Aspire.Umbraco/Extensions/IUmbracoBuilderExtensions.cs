using Microsoft.Extensions.Options;
using Umbraco.Aspire.Umbraco.PostConfigureOptions;
using Umbraco.Cms.Persistence.EFCore;
using Umbraco.StorageProviders.AzureBlob.IO;

namespace Umbraco.Aspire.Umbraco.Extensions;

public static class IUmbracoBuilderExtensions {
    public static IUmbracoBuilder ConfigureAspireServices(this IUmbracoBuilder builder, IHostApplicationBuilder hostApplicationBuilder) {
        // Register the Azure Blob File System with the Umbraco builder
        // and configure it to use the Aspire BlobStorageClient.
        builder
            .AddAzureBlobMediaFileSystem()
            .AddAzureBlobImageSharpCache();

        hostApplicationBuilder.AddAzureBlobContainerClient("umbracomedia");
        hostApplicationBuilder.Services.AddSingleton<IPostConfigureOptions<AzureBlobFileSystemOptions>, PostConfigureAzureBlobFileSystemOptions>();

        if(hostApplicationBuilder.Configuration.GetConnectionString("umbracoaspireredis") is string) {
            hostApplicationBuilder.AddRedisDistributedCache(connectionName: "umbracoaspireredis");
        }

        // Enrich the UmbracoDbContext to report telemetry to Aspire
        hostApplicationBuilder.EnrichSqlServerDbContext<UmbracoDbContext>();
        return builder;
    }
}
