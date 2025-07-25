using System.Diagnostics.Metrics;

namespace Umbraco.Aspire.OpenTelemetry.Meters;

public class uSyncMeter {
    private readonly Counter<int> _uSyncImportedItem;
    private readonly Counter<int> _uSyncExportedItem;
    private readonly Counter<int> _uSyncReportedItem;

    public uSyncMeter(IMeterFactory meterFactory) {
        var meter = meterFactory.Create(UmbracoMeters.uSyncMeter);
        _uSyncImportedItem = meter.CreateCounter<int>("umbraco.usync.imported_item", "count", "Number of items imported by uSync");
        _uSyncExportedItem = meter.CreateCounter<int>("umbraco.usync.exported_item", "count", "Number of items exported by uSync");
        _uSyncReportedItem = meter.CreateCounter<int>("umbraco.usync.reported_item", "count", "Number of items reported by uSync");
    }

    public void ImportedItem(int quantity) {
        _uSyncImportedItem.Add(quantity);
    }

    public void ExportedItem(int quantity) {
        _uSyncExportedItem.Add(quantity);
    }

    public void ReportedItem(int quantity) {
        _uSyncReportedItem.Add(quantity);
    }
}
