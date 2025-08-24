using Umbraco.Aspire.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

builder.DeployAsAppService("umbracoaspireappserviceplan", "B2", "Basic");
//builder.AddAzureContainerAppEnvironment("");

var umbraco = builder.AddUmbracoProject<Projects.Umbraco_Aspire_Umbraco>("umbracoaspireapp");

builder
    .AddAzureSql(umbraco, "umbracoaspiresql", "umbracoaspiresqldb", "Basic", "Basic")
    .AddAzureStorage(umbraco, "umbracoaspirestorage", "umbracomedia")
    .AddAzureKeyVault(umbraco, "umbracoaspirekeyvault")
    .AddApplicationInsights(umbraco, "umbracoaspireinsights")
    .AddRedisCache(umbraco, "umbracoaspireredis")
    .AddFrontendProject(umbraco, "umbracoaspirefrontend");

builder.Build().Run();
