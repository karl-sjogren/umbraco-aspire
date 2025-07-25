using Serilog;
using Serilog.Configuration;

namespace Umbraco.Aspire.Umbraco;

public class DefaultLoggerSettings : ILoggerSettings {
    public void Configure(LoggerConfiguration loggerConfiguration) {
        loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithThreadName()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName();
    }
}
