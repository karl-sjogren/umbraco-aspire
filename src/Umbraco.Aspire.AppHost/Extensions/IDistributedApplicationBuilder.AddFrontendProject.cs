namespace Umbraco.Aspire.AppHost.Extensions;

public static partial class IDistributedApplicationBuilderExtensions {
    public static IDistributedApplicationBuilder AddFrontendProject(
            this IDistributedApplicationBuilder builder,
            IResourceBuilder<ProjectResource> umbracoProject,
            string frontendProjectName) {
        if(builder.ExecutionContext.IsRunMode) {
            //var frontend = builder.AddNpmApp("frontend", "../Umbraco.Aspire.Frontend", "dev");

            //umbraco.WaitFor(frontend);
        } else if(builder.ExecutionContext.IsPublishMode) {
            // Build the frontend in publish mode?
        }

        return builder;
    }
}
