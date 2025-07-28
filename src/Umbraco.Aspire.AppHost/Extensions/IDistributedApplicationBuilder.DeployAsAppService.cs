using Azure.Provisioning.AppService;

namespace Umbraco.Aspire.AppHost.Extensions;

public static partial class IDistributedApplicationBuilderExtensions {
    public static IDistributedApplicationBuilder DeployAsAppService(this IDistributedApplicationBuilder builder, string environmentName, string? skuName = null, string? skuTier = null) {
        builder.AddAzureAppServiceEnvironment(environmentName)
            .ConfigureInfrastructure(infra => {
                var azureResources = infra.GetProvisionableResources();
                var appServicePlan = azureResources.OfType<AppServicePlan>().Single();

                appServicePlan.Sku = new AppServiceSkuDescription {
                    Name = skuName ?? "B2",
                    Tier = skuTier ?? "Basic",
                    Capacity = 1
                };
            });
        return builder;
    }
}
