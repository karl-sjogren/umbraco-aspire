namespace Umbraco.Aspire.AppHost.Extensions;

public static partial class IDistributedApplicationBuilderExtensions {
    public static IDistributedApplicationBuilder AddAzureKeyVault(
            this IDistributedApplicationBuilder builder,
            IResourceBuilder<ProjectResource> umbracoProject,
            string keyVaultName) {
        // Run a local Key Vault emulator in run mode
        if(builder.ExecutionContext.IsRunMode) {
            /*var keyVault = builder
                .AddDockerfile(keyVaultName, "docker/keyvault")
                .WithEndpoint(4997, 4997, "https")
                .WithExternalHttpEndpoints()
                .WithHttpHealthCheck("/token")
                .WithBindMount(source: Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.aspire/keyvault/", "/certs");

            umbracoProject
                .WaitFor(keyVault)
                .WithEnvironment("ConnectionStrings__keyvault", ""); // TODO Figure out
                */
        } else if(builder.ExecutionContext.IsPublishMode) {
            var keyVault = builder.AddAzureKeyVault(keyVaultName);

            umbracoProject.WithReference(keyVault);
        }

        return builder;
    }
}
