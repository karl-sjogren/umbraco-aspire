using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;

namespace Umbraco.Aspire.OpenTelemetry;

public class AspireOpenTelemetryLoggerSettings : ILoggerSettings {
    private readonly IConfiguration _configuration;

    public AspireOpenTelemetryLoggerSettings(IConfiguration configuration) {
        _configuration = configuration;
    }

    public void Configure(LoggerConfiguration loggerConfiguration) {
        loggerConfiguration
            .WriteTo.OpenTelemetry(options => {
                options.Endpoint = _configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
                var headers = _configuration["OTEL_EXPORTER_OTLP_HEADERS"]?.Split(',') ?? [];
                foreach(var header in headers) {
                    var (key, value) = header.Split('=') switch {
                        [string k, string v] => (k, v),
                        var v => throw new Exception($"Invalid header format {v}")
                    };

                    options.Headers.Add(key, value);
                }

                options.ResourceAttributes.Add("service.name", _configuration["OTEL_SERVICE_NAME"] ?? "Umbraco.Aspire.OpenTelemetry");

                //To remove the duplicate issue, we can use the below code to get the key and value from the configuration

                var (otelResourceAttribute, otelResourceAttributeValue) = _configuration["OTEL_RESOURCE_ATTRIBUTES"]?.Split('=') switch {
                    [string k, string v] => (k, v),
                    _ => throw new Exception($"Invalid header format {_configuration["OTEL_RESOURCE_ATTRIBUTES"]}")
                };

                options.ResourceAttributes.Add(otelResourceAttribute, otelResourceAttributeValue);
            });
    }
}
