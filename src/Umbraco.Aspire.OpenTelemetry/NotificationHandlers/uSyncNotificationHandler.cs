using Microsoft.Extensions.DependencyInjection;
using Umbraco.Aspire.OpenTelemetry.Meters;
using Umbraco.Cms.Core.Events;
using uSync.BackOffice;

namespace Umbraco.Aspire.OpenTelemetry.NotificationHandlers;

public class uSyncNotificationHandler :
        INotificationHandler<uSyncImportedItemNotification>,
        INotificationHandler<uSyncExportedItemNotification>,
        INotificationHandler<uSyncReportedItemNotification> {
    private readonly uSyncMeter _meter;

    public uSyncNotificationHandler(uSyncMeter meter) {
        _meter = meter;
    }

    internal static void RegisterNotificationHandlers(IUmbracoBuilder builder) {
        builder.Services.AddSingleton<uSyncMeter>();

        builder.AddNotificationHandler<uSyncImportedItemNotification, uSyncNotificationHandler>();
        builder.AddNotificationHandler<uSyncExportedItemNotification, uSyncNotificationHandler>();
        builder.AddNotificationHandler<uSyncReportedItemNotification, uSyncNotificationHandler>();
    }

    public void Handle(uSyncImportedItemNotification notification) {
        _meter.ImportedItem(1);
    }

    public void Handle(uSyncExportedItemNotification notification) {
        _meter.ExportedItem(1);
    }

    public void Handle(uSyncReportedItemNotification notification) {
        _meter.ReportedItem(1);
    }
}
