namespace Umbraco.Aspire.AppHost.Extensions;

public static partial class IDistributedApplicationBuilderExtensions {
    public static IDistributedApplicationBuilder AddApplicationInsights(
            this IDistributedApplicationBuilder builder,
            IResourceBuilder<ProjectResource> umbracoProject,
            string applicationInsightsName) {
        // Only add when publishing, as we don't need it in run mode
        if(builder.ExecutionContext.IsPublishMode) {
            var insights = builder.AddAzureApplicationInsights(applicationInsightsName);

            umbracoProject.WithReference(insights);
        }

        return builder;
    }
}
