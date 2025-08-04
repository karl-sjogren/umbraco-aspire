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
            });

        var blobContainer = storage.AddBlobContainer(containerName, blobContainerName: containerName);

        umbracoProject
            .WithReference(blobContainer)
            .WaitFor(blobContainer)
            .WithEnvironment("Umbraco__Storage__AzureBlob__Media__ConnectionString", blobContainer.Resource)
            .WithEnvironment("Umbraco__Storage__AzureBlob__Media__ContainerName", blobContainer.Resource.Name);

        return builder;
    }
}
