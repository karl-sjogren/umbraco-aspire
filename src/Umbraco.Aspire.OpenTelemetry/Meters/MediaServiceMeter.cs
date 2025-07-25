using System.Diagnostics.Metrics;

namespace Umbraco.Aspire.OpenTelemetry.Meters;

public class MediaServiceMeter {
    private readonly Counter<int> _mediaSaved;
    private readonly Counter<int> _mediaMoved;
    private readonly Counter<int> _mediaMovedToRecycleBin;
    private readonly Counter<int> _mediaDeleted;

    public MediaServiceMeter(IMeterFactory meterFactory) {
        var meter = meterFactory.Create(UmbracoMeters.MediaServiceMeter);
        _mediaSaved = meter.CreateCounter<int>("umbraco.media.saved", "count", "Number of media items saved");
        _mediaMoved = meter.CreateCounter<int>("umbraco.media.moved", "count", "Number of media items moved");
        _mediaMovedToRecycleBin = meter.CreateCounter<int>("umbraco.media.moved_to_recycle_bin", "count", "Number of media items moved to recycle bin");
        _mediaDeleted = meter.CreateCounter<int>("umbraco.media.deleted", "count", "Number of media items deleted");
    }

    public void MediaSaved(int quantity) {
        _mediaSaved.Add(quantity);
    }

    public void MediaMoved(int quantity) {
        _mediaMoved.Add(quantity);
    }

    public void MediaMovedToRecycleBin(int quantity) {
        _mediaMovedToRecycleBin.Add(quantity);
    }

    public void MediaDeleted(int quantity) {
        _mediaDeleted.Add(quantity);
    }
}
