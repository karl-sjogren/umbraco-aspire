using System.IO.Abstractions;
using System.IO.Packaging;
using Aspire.Hosting.Azure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Dac;

namespace Umbraco.Aspire.AppHost.Commands;

public static class DownloadAzureSqlCommandsExtensions {
    private const string _exportFilename = "./tmp/azure-export.bacpac";
    private const string _importUserId = "sax";

    public static IResourceBuilder<AzureSqlDatabaseResource> WithDownloadAzureSqlCommands(
            this IResourceBuilder<AzureSqlDatabaseResource> builder,
            string serverName,
            string databaseName) {
        if(!builder.ApplicationBuilder.ExecutionContext.IsRunMode) {
            return builder;
        }

        builder
            .WithDownloadCommand(serverName, databaseName)
            .WithImportCommand();

        return builder;
    }

    private static IResourceBuilder<AzureSqlDatabaseResource> WithDownloadCommand(this IResourceBuilder<AzureSqlDatabaseResource> builder, string serverName, string databaseName) {
        var commandOptions = new CommandOptions {
            UpdateState = (context) => context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
                ? ResourceCommandState.Enabled
                : ResourceCommandState.Disabled,
            IconName = "ArrowCircleDown",
            IconVariant = IconVariant.Filled
        };

        builder.WithCommand(
            name: "azuresql-download-bacpac",
            displayName: "Download bacpac from Azure",
            executeCommand: context => OnDownloadBacpacFromAzureAsync(builder, context, serverName, databaseName),
            commandOptions: commandOptions);

        return builder;
    }

    private static Task<ExecuteCommandResult> OnDownloadBacpacFromAzureAsync(
            IResourceBuilder<AzureSqlDatabaseResource> builder,
            ExecuteCommandContext context,
            string serverName,
            string databaseName) {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();

        if(string.IsNullOrEmpty(serverName)) {
            return Task.FromResult(CommandResults.Failure("Server name must be specified."));
        }

        var connectionString = $"Server=tcp:{serverName}.database.windows.net,1433;Initial Catalog={databaseName};Authentication=Active Directory Interactive;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";

        var service = new DacServices(connectionString);

        service.Message += (sender, e) => {
            logger.LogInformation("DacServices Message: {Message}", e.Message);
        };

        var hasError = false;
        service.ProgressChanged += (sender, e) => {
            if(e.Status == DacOperationStatus.Faulted) {
                logger.LogError("DacServices operation faulted: {Message}", e.Message);
                hasError = true;
            } else if(e.Status == DacOperationStatus.Cancelled) {
                logger.LogWarning("DacServices operation cancelled: {Message}", e.Message);
                hasError = true;
            } else {
                logger.LogDebug("DacServices operation progress: {Message}", e.Message);
            }
        };

        var options = new DacExportOptions {
            CompressionOption = CompressionOption.Normal
        };

        logger.LogInformation("Exporting bacpac for database {DatabaseName} from server {ServerName}...", databaseName, serverName);

        service.ExportBacpac(_exportFilename, databaseName, options, null, context.CancellationToken);

        if(hasError) {
            return Task.FromResult(CommandResults.Failure("Export failed."));
        }

        return Task.FromResult(CommandResults.Success());
    }

    private static IResourceBuilder<AzureSqlDatabaseResource> WithImportCommand(this IResourceBuilder<AzureSqlDatabaseResource> builder) {
        var commandOptions = new CommandOptions {
            UpdateState = (context) => context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
                ? ResourceCommandState.Enabled
                : ResourceCommandState.Disabled,
            IconName = "ArrowImport",
            IconVariant = IconVariant.Filled
        };

        builder.WithCommand(
            name: "azuresql-import-bacpac",
            displayName: "Import downloaded bacpac",
            executeCommand: async context => {
                var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();
                return await OnImportBacpacAsync(builder.Resource, logger, context.CancellationToken);
            },
            commandOptions: commandOptions);

        return builder;
    }

    private static async Task<ExecuteCommandResult> OnImportBacpacAsync(
            IResourceWithConnectionString resource,
            ILogger logger,
            CancellationToken cancellationToken) {
        var fileSystem = new FileSystem();
        if(!fileSystem.File.Exists(_exportFilename)) {
            return CommandResults.Failure("Bacpac file not found, use the download command first.");
        }

        var connectionString = await resource.GetConnectionStringAsync(cancellationToken);

        if(string.IsNullOrEmpty(connectionString)) {
            return CommandResults.Failure("Connection string is not available.");
        }

        await RecreateDatabaseAsync(connectionString, logger, cancellationToken);

        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString) {
            UserID = _importUserId
        };

        var service = new DacServices(connectionString);
        var package = BacPackage.Load(_exportFilename);

        service.Message += (sender, e) => {
            logger.LogInformation("DacServices Message: {Message}", e.Message);
        };

        var hasError = false;
        service.ProgressChanged += (sender, e) => {
            if(e.Status == DacOperationStatus.Faulted) {
                logger.LogError("DacServices operation faulted: {Message}", e.Message);
                hasError = true;
            } else if(e.Status == DacOperationStatus.Cancelled) {
                logger.LogWarning("DacServices operation cancelled: {Message}", e.Message);
                hasError = true;
            } else {
                logger.LogDebug("DacServices operation progress: {Message}", e.Message);
            }
        };

        logger.LogInformation("Importing bacpac to database {DatabaseName} on local server...", connectionStringBuilder.InitialCatalog);

        service.ImportBacpac(package, connectionStringBuilder.InitialCatalog);

        if(hasError) {
            return CommandResults.Failure("Import failed.");
        }

        return CommandResults.Success();
    }

    private static async Task RecreateDatabaseAsync(string connectionString, ILogger logger, CancellationToken cancellationToken) {
        logger.LogInformation("Recreating database...");

        await EnsureUserCreatedAsync(connectionString, cancellationToken);

        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString) {
            UserID = _importUserId
        };

        logger.LogInformation("Starting database import...");

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        var databaseName = connectionStringBuilder.InitialCatalog;

        command.CommandText = $"use [master]; ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
        await command.ExecuteNonQueryAsync(cancellationToken);

        command.CommandText = $"DROP DATABASE IF EXISTS [{databaseName}]; CREATE DATABASE [{databaseName}];";
        await command.ExecuteNonQueryAsync(cancellationToken);

        command.CommandText = $"ALTER DATABASE [{databaseName}] SET MULTI_USER;";
        await command.ExecuteNonQueryAsync(cancellationToken);

        logger.LogInformation("Database import completed.");
    }

    private static async Task EnsureUserCreatedAsync(string connectionString, CancellationToken cancellationToken) {
        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = $"""
IF SUSER_ID('{_importUserId}') IS NULL
    CREATE LOGIN [{_importUserId}] WITH PASSWORD = '{connectionStringBuilder.Password}!';

-- Check database user
IF DATABASE_PRINCIPAL_ID('{_importUserId}') IS NULL
BEGIN
    CREATE USER [{_importUserId}] FOR LOGIN [{_importUserId}];
END

ALTER ROLE [db_owner] ADD MEMBER [{_importUserId}]
""";

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
