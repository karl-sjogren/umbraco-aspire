using Aspire.Hosting.Azure;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Umbraco.Aspire.AppHost.Commands;

public static class CopyFromAzureStorageCommandExtensions {
    public static IResourceBuilder<AzureStorageEmulatorResource> WithCopyFromAzureStorageCommand(
            this IResourceBuilder<AzureStorageEmulatorResource> builder,
            string storageAccount,
            string containerName,
            string? prefix = null) {
        if(!builder.ApplicationBuilder.ExecutionContext.IsRunMode) {
            return builder;
        }

        var commandOptions = new CommandOptions {
            UpdateState = OnUpdateResourceState,
            IconName = "DrawerArrowDownload",
            IconVariant = IconVariant.Regular
        };

        builder.WithCommand(
            name: "copy-blobs-from-azure",
                displayName: "Copy blobs from Azure",
                executeCommand: context => OnRunCopyBlobsFromAzureCommandAsync(context, storageAccount, containerName, prefix),
                commandOptions: commandOptions);

        return builder;
    }

    private static async Task<ExecuteCommandResult> OnRunCopyBlobsFromAzureCommandAsync(
            ExecuteCommandContext context,
            string storageAccount,
            string containerName,
            string? prefix) {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Copying blobs from Azure Storage to Azurite...");

        var azuritePort = 60093;

        try {
            await CopyBlobsToAzuriteAsync(context, storageAccount, containerName, azuritePort, prefix);
        } catch(Exception ex) {
            if(ex is not OperationCanceledException) {
                logger.LogError(ex, "An error occurred while copying blobs from Azure Storage to Azurite.");
                return CommandResults.Failure(ex);
            }
        }

        return CommandResults.Success();
    }

    private static async Task CopyBlobsToAzuriteAsync(
            ExecuteCommandContext context,
            string storageAccount,
            string containerName,
            int azuritePort,
            string? prefix) {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var cancellationToken = context.CancellationToken;

        var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions {
            ExcludeInteractiveBrowserCredential = false
        });

        var blobServiceClient = new BlobServiceClient(
            new Uri($"https://{storageAccount}.blob.core.windows.net/"),
            credential);

        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

        var azuriteClient = new BlobServiceClient(
            new Uri($"http://127.0.0.1:{azuritePort}/" + AzuriteConstants.AzuriteStorageAccountName),
            new StorageSharedKeyCredential(AzuriteConstants.AzuriteStorageAccountName, AzuriteConstants.AzuriteAccountKey));

        var azuriteContainer = azuriteClient.GetBlobContainerClient(containerName);

        await Parallel.ForEachAsync(blobContainerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken), async (blob, cancellationToken) => {
            var blobClient = blobContainerClient.GetBlobClient(blob.Name);

            var targetReference = azuriteContainer.GetBlockBlobClient(blob.Name);

            if(await targetReference.ExistsAsync(cancellationToken)) {
                var targetProperties = await targetReference.GetPropertiesAsync(cancellationToken: cancellationToken);
                var targetLength = targetProperties.Value.ContentLength;

                var sourceProperties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
                var sourceLength = sourceProperties.Value.ContentLength;

                if(targetLength == sourceLength) {
                    logger.LogDebug("Blob {blobName} already exists in Azurite with the same length, skipping.", blob.Name);
                    return;
                }
            }

            logger.LogInformation("Copying blob: {blobName}", blob.Name);
            await using var targetStream = await targetReference.OpenWriteAsync(true, cancellationToken: cancellationToken);
            await blobClient.DownloadToAsync(targetStream, cancellationToken: cancellationToken);
        });
    }

    private static ResourceCommandState OnUpdateResourceState(UpdateCommandStateContext context) {
        return context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
            ? ResourceCommandState.Enabled
            : ResourceCommandState.Disabled;
    }

    private sealed class AzuriteConstants {
        public const string AzuriteStorageAccountName = "devstoreaccount1";
        public const string AzuriteAccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";
    }
}
