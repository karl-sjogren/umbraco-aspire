namespace Umbraco.Aspire.AppHost.Extensions;

public static partial class IDistributedApplicationBuilderExtensions {
    public static IDistributedApplicationBuilder AddAzureStorage(
            this IDistributedApplicationBuilder builder,
            IResourceBuilder<ProjectResource> umbracoProject,
            string accountName,
            string blobStorageName,
            string containerName) {
        var storage = builder
            .AddAzureStorage(accountName)
            .RunAsEmulator(azurite => {
                azurite.WithDataVolume();
                azurite.WithLifetime(ContainerLifetime.Persistent);
            });

        // Keep a reference to the storage resource so we can get a connection string
        // without the media container later for Umbraco to use
        var blobStorage = storage.AddBlobs(blobStorageName);

        // Create a media container in the blob storage and keep a reference to it
        // so we can use it with the Aspire.Azure.Storage.Blobs package
        var mediaContainer = blobStorage.AddBlobContainer(containerName);

        umbracoProject
            .WithReference(mediaContainer)
            .WaitFor(mediaContainer)
            .WithEnvironment("Umbraco__Storage__AzureBlob__Media__ConnectionString", blobStorage) // Use the blob storage resource and not the media container directly to get a proper connection string
            .WithEnvironment("Umbraco__Storage__AzureBlob__Media__ContainerName", mediaContainer.Resource.Name); // Just get the container name from the media container resource

        return builder;
    }
}
