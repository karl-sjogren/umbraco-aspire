using Microsoft.Extensions.DependencyInjection;
using Serilog.Configuration;
using Umbraco.Aspire.OpenTelemetry.NotificationHandlers;
using Umbraco.Cms.Core.Composing;

namespace Umbraco.Aspire.OpenTelemetry;

public class UmbracoOpenTelemetryComposer : IComposer {
    public void Compose(IUmbracoBuilder builder) {
        builder.Services.AddSingleton<ILoggerSettings, AspireOpenTelemetryLoggerSettings>();

        builder.Services.AddOpenTelemetry().WithMetrics(metrics => {
            metrics.AddMeter(UmbracoMeters.ContentServiceMeter);
            metrics.AddMeter(UmbracoMeters.MediaServiceMeter);
            metrics.AddMeter(UmbracoMeters.uSyncMeter);
        });

        ContentServiceNotificationHandler.RegisterNotificationHandlers(builder);
        MediaServiceNotificationHandler.RegisterNotificationHandlers(builder);
        uSyncNotificationHandler.RegisterNotificationHandlers(builder);
    }
}
