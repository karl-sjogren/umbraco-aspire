using Azure.Provisioning.Sql;

namespace Umbraco.Aspire.AppHost.Extensions;

public static partial class IDistributedApplicationBuilderExtensions {
    public static IDistributedApplicationBuilder AddAzureSql(
            this IDistributedApplicationBuilder builder,
            IResourceBuilder<ProjectResource> umbracoProject,
            string serverName,
            string databaseName,
            string? skuName = null,
            string? skuTier = null) {
        var azureSql = builder.AddAzureSqlServer(serverName)
            .ConfigureInfrastructure(infra => {
                var azureResources = infra.GetProvisionableResources();
                var azureDb = azureResources.OfType<SqlDatabase>().Single();
                azureDb.UseFreeLimit = false;
                azureDb.Sku = new SqlSku() {
                    Name = skuName ?? "Basic",
                    Tier = skuTier ?? "Basic",
                    Capacity = 5
                };
            });

        if(builder.ExecutionContext.IsRunMode) {
            azureSql
                .RunAsContainer(x => {
                    x.WithDataVolume();
                    x.WithLifetime(ContainerLifetime.Persistent);
                });
        }

        var azureSqlDatabase = azureSql
            .AddDatabase(databaseName);

        umbracoProject
            .WithReference(azureSqlDatabase)
            .WaitFor(azureSqlDatabase)
            .WithEnvironment("ConnectionStrings__umbracoDbDSN", azureSqlDatabase);

        return builder;
    }
}
