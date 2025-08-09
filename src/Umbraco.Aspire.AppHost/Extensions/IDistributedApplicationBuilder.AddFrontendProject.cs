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
        } else if(builder.ExecutionContext.IsPublishMode) {
            // Not sure this is the right way to do this, might want to put this in the project file instead
            var frontend = builder.AddNpmApp(frontendProjectName, "../Umbraco.Aspire.Frontend", "build");

            umbracoProject.WaitFor(frontend);
        }

        return builder;
    }
}
