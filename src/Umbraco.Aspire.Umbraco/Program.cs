using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Umbraco.Cms.Persistence.EFCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

if(builder.Configuration.GetConnectionString("umbraco-aspire-keyvault") is string keyVaultConnectionString) {
    var options = new SecretClientOptions { DisableChallengeResourceVerification = true, Diagnostics = { IsLoggingEnabled = true } };
    var secretClient = new SecretClient(new Uri(keyVaultConnectionString), new DefaultAzureCredential(), options);

    builder.Configuration.AddAzureKeyVault(secretClient, new AzureKeyVaultConfigurationOptions());
} else if(builder.Configuration["KeyVaultName"] is string keyVaultName) {
    builder.Configuration.AddAzureKeyVault(new Uri($"https://{keyVaultName}.vault.azure.net/"), new DefaultAzureCredential());
}

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .AddAzureBlobMediaFileSystem()
    .AddAzureBlobImageSharpCache()
    .Build();

builder.EnrichSqlServerDbContext<UmbracoDbContext>();

var app = builder.Build();

await app.BootUmbracoAsync();

app.UseUmbraco()
    .WithMiddleware(u => {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u => {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
