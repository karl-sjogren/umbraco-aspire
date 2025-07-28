namespace Umbraco.Aspire.AppHost.Extensions;

public static partial class IDistributedApplicationBuilderExtensions {
    public static IResourceBuilder<ProjectResource> AddUmbracoProject<TProject>(this IDistributedApplicationBuilder builder, string projectName) where TProject : IProjectMetadata, new() {
        return builder.AddProject<Projects.Umbraco_Aspire_Umbraco>(projectName)
            .WithEnvironment("Umbraco__CMS__Unattended__InstallUnattended", bool.TrueString)
            .WithEnvironment("Umbraco__CMS__Unattended__UnattendedUserName", "jack.sparrow")
            .WithEnvironment("Umbraco__CMS__Unattended__UnattendedUserEmail", "jack.sparrow@pirates.com")
            .WithEnvironment("Umbraco__CMS__Unattended__UnattendedUserPassword", "password123");
    }
}
