using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Data.SqlClient;
using Serilog;
using Serilog.Configuration;
using Shorthand.Vite;
using Umbraco.Aspire.Umbraco;
using Umbraco.Aspire.Umbraco.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSerilog();
builder.Services.AddSingleton<ILoggerSettings, DefaultLoggerSettings>();

var sqlConnectionString = builder.Configuration.GetConnectionString("umbracoDbDSN");
using(var connection = new SqlConnection(sqlConnectionString)) {
    await connection.OpenAsync();
    var serverVersion = connection.ServerVersion;
    Console.WriteLine($"Connected to SQL Server version: {serverVersion}");
}

if(builder.Configuration.GetConnectionString("umbraco-aspire-keyvault") is string keyVaultConnectionString) {
    var options = new SecretClientOptions { DisableChallengeResourceVerification = true, Diagnostics = { IsLoggingEnabled = true } };
    var secretClient = new SecretClient(new Uri(keyVaultConnectionString), new DefaultAzureCredential(), options);

    builder.Configuration.AddAzureKeyVault(secretClient, new AzureKeyVaultConfigurationOptions());
} else if(builder.Configuration["KeyVaultName"] is string keyVaultName) {
    builder.Configuration.AddAzureKeyVault(new Uri($"https://{keyVaultName}.vault.azure.net/"), new DefaultAzureCredential());
}

if(builder.Environment.IsDevelopment()) {
    var configurationDebugView = builder.Configuration.GetDebugView();
    Console.WriteLine($"Configuration Debug View: {configurationDebugView}");
}

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .ConfigureAspireServices(builder)
    .Build();

builder.Services.AddVite(options => {
    options.ManifestFileName = ".vite/manifest.json";
    options.Port = 5010;
    options.Https = true;
});

builder.AddRedisDistributedCache(connectionName: "umbraco-aspire-redis");

var app = builder.Build();

await app.BootUmbracoAsync();

app.UseSerilogRequestLogging();

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
