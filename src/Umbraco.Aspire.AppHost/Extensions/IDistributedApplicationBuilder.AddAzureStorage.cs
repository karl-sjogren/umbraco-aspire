using Umbraco.Aspire.AppHost.Commands;

namespace Umbraco.Aspire.AppHost.Extensions;

public static partial class IDistributedApplicationBuilderExtensions {
    public static IDistributedApplicationBuilder AddAzureStorage(
            this IDistributedApplicationBuilder builder,
            IResourceBuilder<ProjectResource> umbracoProject,
            string accountName,
            string containerName) {
        var storage = builder
            .AddAzureStorage(accountName)
            .RunAsEmulator(azurite => {
                azurite.WithDataVolume();
                azurite.WithLifetime(ContainerLifetime.Persistent);
                azurite.WithCopyFromAzureStorageCommand(accountName, containerName, prefix: "media/");
            });

        var blobContainer = storage.AddBlobContainer(containerName, blobContainerName: containerName);

        umbracoProject
            .WithReference(blobContainer)
            .WaitFor(blobContainer);

        return builder;
    }
}
