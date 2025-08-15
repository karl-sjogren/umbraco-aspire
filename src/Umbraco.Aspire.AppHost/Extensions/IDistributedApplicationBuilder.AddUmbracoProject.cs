namespace Umbraco.Aspire.AppHost.Extensions;

public static partial class IDistributedApplicationBuilderExtensions {
    public static IResourceBuilder<ProjectResource> AddUmbracoProject<TProject>(this IDistributedApplicationBuilder builder, string projectName) where TProject : IProjectMetadata, new() {
        var project = builder.AddProject<Projects.Umbraco_Aspire_Umbraco>(projectName)
            .WithExternalHttpEndpoints()
            // Currently not working, https://github.com/dotnet/aspire/issues/10983
            //.WithUrl("/umbraco", "Umbraco Backoffice")
            .WithAnnotation(new ResourceUrlsCallbackAnnotation(c => {
                c.Urls.Add(new() { Url = "/umbraco", DisplayText = "Umbraco Backoffice", Endpoint = c.GetEndpoint("https") });
            }))
            .WithEnvironment("Umbraco__CMS__Unattended__InstallUnattended", bool.TrueString)
            .WithEnvironment("Umbraco__CMS__Unattended__UnattendedUserName", "jack.sparrow")
            .WithEnvironment("Umbraco__CMS__Unattended__UnattendedUserEmail", "jack.sparrow@pirates.com")
            .WithEnvironment("Umbraco__CMS__Unattended__UnattendedUserPassword", "password123");

        return project;
    }
}
