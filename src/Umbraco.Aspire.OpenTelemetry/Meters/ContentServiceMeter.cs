using System.Diagnostics.Metrics;

namespace Umbraco.Aspire.OpenTelemetry.Meters;

public class ContentServiceMeter {
    private readonly Counter<int> _contentSaved;
    private readonly Counter<int> _contentPublished;
    private readonly Counter<int> _contentUnpublished;
    private readonly Counter<int> _contentCopied;
    private readonly Counter<int> _contentMoved;
    private readonly Counter<int> _contentMovedToRecycleBin;
    private readonly Counter<int> _contentDeleted;
    private readonly Counter<int> _contentRolledBack;
    private readonly Counter<int> _contentSentToPublish;
    private readonly Counter<int> _contentEmptiedRecycleBin;
    private readonly Counter<int> _contentEmptiedRecycleBinCount;
    private readonly Counter<int> _contentSavedBlueprint;
    private readonly Counter<int> _contentDeletedBlueprint;

    public ContentServiceMeter(IMeterFactory meterFactory) {
        var meter = meterFactory.Create(UmbracoMeters.ContentServiceMeter);
        _contentSaved = meter.CreateCounter<int>("umbraco.content.saved", "count", "Number of content items saved");
        _contentPublished = meter.CreateCounter<int>("umbraco.content.published", "count", "Number of content items published");
        _contentUnpublished = meter.CreateCounter<int>("umbraco.content.unpublished", "count", "Number of content items unpublished");
        _contentCopied = meter.CreateCounter<int>("umbraco.content.copied", "count", "Number of content items copied");
        _contentMoved = meter.CreateCounter<int>("umbraco.content.moved", "count", "Number of content items moved");
        _contentMovedToRecycleBin = meter.CreateCounter<int>("umbraco.content.moved_to_recycle_bin", "count", "Number of content items moved to recycle bin");
        _contentDeleted = meter.CreateCounter<int>("umbraco.content.deleted", "count", "Number of content items deleted");
        _contentRolledBack = meter.CreateCounter<int>("umbraco.content.rolled_back", "count", "Number of content items rolled back");
        _contentSentToPublish = meter.CreateCounter<int>("umbraco.content.sent_to_publish", "count", "Number of content items sent to publish");
        _contentEmptiedRecycleBin = meter.CreateCounter<int>("umbraco.content.emptied_recycle_bin", "count", "Number of content items emptied from recycle bin");
        _contentEmptiedRecycleBinCount = meter.CreateCounter<int>("umbraco.content.emptied_recycle_bin_count", "count", "Total count of content items emptied from recycle bin");
        _contentSavedBlueprint = meter.CreateCounter<int>("umbraco.content.saved_blueprint", "count", "Number of content blueprints saved");
        _contentDeletedBlueprint = meter.CreateCounter<int>("umbraco.content.deleted_blueprint", "count", "Number of content blueprints deleted");
    }

    public void ContentSaved(int quantity) {
        _contentSaved.Add(quantity);
    }

    public void ContentPublished(int quantity) {
        _contentPublished.Add(quantity);
    }

    public void ContentUnpublished(int quantity) {
        _contentUnpublished.Add(quantity);
    }

    public void ContentCopied(int quantity) {
        _contentCopied.Add(quantity);
    }

    public void ContentMoved(int quantity) {
        _contentMoved.Add(quantity);
    }

    public void ContentMovedToRecycleBin(int quantity) {
        _contentMovedToRecycleBin.Add(quantity);
    }

    public void ContentDeleted(int quantity) {
        _contentDeleted.Add(quantity);
    }

    public void ContentRolledBack(int quantity) {
        _contentRolledBack.Add(quantity);
    }

    public void ContentSentToPublish(int quantity) {
        _contentSentToPublish.Add(quantity);
    }

    public void ContentEmptiedRecycleBin(int quantity) {
        _contentEmptiedRecycleBin.Add(quantity);
    }

    public void ContentEmptiedRecycleBinCount(int quantity) {
        _contentEmptiedRecycleBinCount.Add(quantity);
    }

    public void ContentSavedBlueprint(int quantity) {
        _contentSavedBlueprint.Add(quantity);
    }

    public void ContentDeletedBlueprint(int quantity) {
        _contentDeletedBlueprint.Add(quantity);
    }
}
