using System.Diagnostics;
using System.Text;
using Aspire.Hosting.Azure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Umbraco.Aspire.AppHost.Extensions;

public static class AzureSqlDatabaseResourceBuilderExtensions {
    public static IResourceBuilder<AzureSqlDatabaseResource> WithVSCodeMSSQLCommand(
            this IResourceBuilder<AzureSqlDatabaseResource> builder,
            bool useInsider = false) {
        var commandOptions = new CommandOptions {
            UpdateState = OnUpdateResourceState,
            IconName = "Code",
            IconVariant = IconVariant.Filled
        };

        builder.WithCommand(
            name: "launch-vscode-mssql",
                displayName: useInsider ? "Launch VSCode Insider MSSQL" : "Launch VSCode MSSQL",
                executeCommand: context => OnRunLaunchVscodeMssqlCommandAsync(builder, context, useInsider),
                commandOptions: commandOptions);

        return builder;
    }

    private static async Task<ExecuteCommandResult> OnRunLaunchVscodeMssqlCommandAsync(
            IResourceBuilder<AzureSqlDatabaseResource> builder,
            ExecuteCommandContext context,
            bool useInsider) {
        /*
        var connectionString = await builder.Resource.() ??
            throw new InvalidOperationException(
                $"Unable to get the '{context.ResourceName}' connection string.");

        await using var connection = ConnectionMultiplexer.Connect(connectionString);
        var database = connection.GetDatabase();
        await database.ExecuteAsync("FLUSHALL");*/

        var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();

        var connectionStringWithResource = builder.Resource as IResourceWithConnectionString;
        var connectionString = await connectionStringWithResource.GetConnectionStringAsync();

        if(string.IsNullOrEmpty(connectionString)) {
            return CommandResults.Failure("Connection string is not available.");
        }

        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"server={Uri.EscapeDataString(connectionStringBuilder.DataSource)};");
        stringBuilder.Append($"database={Uri.EscapeDataString(connectionStringBuilder.InitialCatalog)};");
        stringBuilder.Append($"authenticationType=SqlLogin;");
        stringBuilder.Append($"trustServerCertificate=true;");
        stringBuilder.Append($"user={Uri.EscapeDataString(connectionStringBuilder.UserID)};");
        stringBuilder.Append($"password={Uri.EscapeDataString(connectionStringBuilder.Password)};");

        var argumentString = stringBuilder.ToString();

        var mssqlExtension = useInsider ? "vscode-insiders://ms-mssql.mssql/connect?" : "vscode://ms-mssql.mssql/connect?";

        logger.LogInformation("Launching VSCode MSSQL with uri: {mssqlExtension}{ConnectionString}", mssqlExtension, argumentString);

        Process.Start(new ProcessStartInfo {
            FileName = $"{mssqlExtension}{argumentString}",
            UseShellExecute = true
        });

        return CommandResults.Success();
    }

    private static ResourceCommandState OnUpdateResourceState(UpdateCommandStateContext context) {
        return context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
            ? ResourceCommandState.Enabled
            : ResourceCommandState.Disabled;
    }
}
