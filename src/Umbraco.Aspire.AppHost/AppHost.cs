using Umbraco.Aspire.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

builder.DeployAsAppService("umbraco-aspire-appservice-plan", "B2", "Basic");

var umbraco = builder.AddUmbracoProject<Projects.Umbraco_Aspire_Umbraco>("umbraco-aspire-umbraco");

builder
    .AddAzureSql(umbraco, "umbraco-aspire-sql", "umbraco-aspire-sql-db", "Basic", "Basic")
    .AddAzureStorage(umbraco, "umbraco-aspire-storage", "umbraco-media")
    //.AddAzureKeyVault(umbraco, "umbraco-aspire-keyvault")
    .AddApplicationInsights(umbraco, "umbraco-aspire-insights")
    .AddRedisCache(umbraco, "umbraco-aspire-redis")
    .AddFrontendProject(umbraco, "umbraco-aspire-frontend");

builder.Build().Run();
