namespace Umbraco.Aspire.AppHost.Extensions;

public static partial class IDistributedApplicationBuilderExtensions {
    public static IDistributedApplicationBuilder AddFrontendProject(
            this IDistributedApplicationBuilder builder,
            IResourceBuilder<ProjectResource> umbracoProject,
            string frontendProjectName) {
        if(builder.ExecutionContext.IsRunMode) {
            var frontend = builder
                .AddNpmApp(frontendProjectName, "../Umbraco.Aspire.Frontend", "dev");

            umbracoProject.WaitFor(frontend);
        }

        return builder;
    }
}
