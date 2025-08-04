using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Umbraco.StorageProviders.AzureBlob.IO;

namespace Umbraco.Aspire.Umbraco.PostConfigureOptions;

public class PostConfigureAzureBlobFileSystemOptions : IPostConfigureOptions<AzureBlobFileSystemOptions> {
    private readonly BlobContainerClient _blobContainerClient;

    public PostConfigureAzureBlobFileSystemOptions(BlobContainerClient blobContainerClient) {
        _blobContainerClient = blobContainerClient;
    }

    public void PostConfigure(string? name, AzureBlobFileSystemOptions options) {
        ArgumentNullException.ThrowIfNull(options);

        options.ContainerName = "not-a-real-container-name";
        options.ConnectionString = "https://not.a.real.storage.account.blob.core.windows.net/";
        options.TryCreateBlobContainerClientUsingUri(_ => _blobContainerClient);
    }
}
