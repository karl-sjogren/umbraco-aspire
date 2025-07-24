using AzureKeyVaultEmulator.Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var azureSql = builder.AddAzureSqlServer("umbraco-aspire-sql");

if(builder.ExecutionContext.IsRunMode) {
    azureSql
        .RunAsContainer(x => {
            x.WithDataVolume();
            x.WithLifetime(ContainerLifetime.Persistent);
        });
}

var storage = builder.AddAzureStorage("umbraco-aspire-storage");

if(builder.ExecutionContext.IsRunMode) {
    storage.RunAsEmulator(azurite => {
        azurite.WithDataVolume();
        azurite.WithLifetime(ContainerLifetime.Persistent);
    });
}

var mediaContainer = storage.AddBlobs("umbraco-aspire-storage-blobs")
    .AddBlobContainer("umbraco-media");

var azureSqlDatabase = azureSql
    .AddDatabase("umbracoDbDSN");

var umbraco = builder.AddProject<Projects.Umbraco_Aspire_Umbraco>("umbraco-aspire-umbraco")
    .WithExternalHttpEndpoints()
    .WithReference(azureSqlDatabase)
    .WaitFor(azureSqlDatabase)
    .WithReference(mediaContainer)
    .WaitFor(mediaContainer)
    .WithEnvironment(context => {
        context.EnvironmentVariables["Umbraco__Storage__AzureBlob__Media__ConnectionString"] = new ConnectionStringReference(mediaContainer.Resource, false);
    })
    .WithEnvironment("Umbraco__Storage__AzureBlob__Media__ContainerName", mediaContainer.Resource.Name)
    .WithEnvironment("umbracoDbDSN_ProviderName", "System.Data.SqlClient")
    .WithEnvironment("Umbraco__CMS__Unattended__InstallUnattended", bool.TrueString)
    .WithEnvironment("Umbraco__CMS__Unattended__UnattendedUserName", "jack.sparrow")
    .WithEnvironment("Umbraco__CMS__Unattended__UnattendedUserEmail", "jack.sparrow@pirates.com")
    .WithEnvironment("Umbraco__CMS__Unattended__UnattendedUserPassword", "password123");

if(builder.ExecutionContext.IsRunMode) {
    var keyvault = builder
        .AddDockerfile("umbraco-aspire-keyvault", "docker/keyvault")
        .WithEndpoint(4997, 4997, "https")
        .WithExternalHttpEndpoints()
        .WithHttpHealthCheck("/token")
        .WithBindMount(source: Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.aspire/keyvault/", "/certs");

    umbraco
        .WaitFor(keyvault)
        .WithEnvironment("ConnectionStrings__keyvault", ""); // TOD Figure out
}

if(builder.ExecutionContext.IsRunMode) {
    //var frontend = builder.AddNpmApp("frontend", "../Umbraco.Aspire.Frontend", "dev");

    //umbraco.WaitFor(frontend);
} else if(builder.ExecutionContext.IsPublishMode) {
    // Build the frontend in publish mode?
}

builder.Build().Run();
