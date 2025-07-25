using Microsoft.Extensions.DependencyInjection;
using Umbraco.Aspire.OpenTelemetry.Meters;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Umbraco.Aspire.OpenTelemetry.NotificationHandlers;

public class ContentServiceNotificationHandler :
        INotificationHandler<ContentSavedNotification>,
        INotificationHandler<ContentPublishedNotification>,
        INotificationHandler<ContentUnpublishedNotification>,
        INotificationHandler<ContentCopiedNotification>,
        INotificationHandler<ContentMovedNotification>,
        INotificationHandler<ContentMovedToRecycleBinNotification>,
        INotificationHandler<ContentDeletedNotification>,
        INotificationHandler<ContentRolledBackNotification>,
        INotificationHandler<ContentSentToPublishNotification>,
        INotificationHandler<ContentEmptiedRecycleBinNotification>,
        INotificationHandler<ContentSavedBlueprintNotification>,
        INotificationHandler<ContentDeletedBlueprintNotification> {
    private readonly ContentServiceMeter _meter;

    public ContentServiceNotificationHandler(ContentServiceMeter meter) {
        _meter = meter;
    }

    internal static void RegisterNotificationHandlers(IUmbracoBuilder builder) {
        builder.Services.AddSingleton<ContentServiceMeter>();

        builder.AddNotificationHandler<ContentSavedNotification, ContentServiceNotificationHandler>();
        builder.AddNotificationHandler<ContentPublishedNotification, ContentServiceNotificationHandler>();
        builder.AddNotificationHandler<ContentUnpublishedNotification, ContentServiceNotificationHandler>();
        builder.AddNotificationHandler<ContentCopiedNotification, ContentServiceNotificationHandler>();
        builder.AddNotificationHandler<ContentMovedNotification, ContentServiceNotificationHandler>();
        builder.AddNotificationHandler<ContentMovedToRecycleBinNotification, ContentServiceNotificationHandler>();
        builder.AddNotificationHandler<ContentDeletedNotification, ContentServiceNotificationHandler>();
        builder.AddNotificationHandler<ContentRolledBackNotification, ContentServiceNotificationHandler>();
        builder.AddNotificationHandler<ContentSentToPublishNotification, ContentServiceNotificationHandler>();
        builder.AddNotificationHandler<ContentEmptiedRecycleBinNotification, ContentServiceNotificationHandler>();
        builder.AddNotificationHandler<ContentSavedBlueprintNotification, ContentServiceNotificationHandler>();
        builder.AddNotificationHandler<ContentDeletedBlueprintNotification, ContentServiceNotificationHandler>();
    }

    public void Handle(ContentSavedNotification notification) {
        _meter.ContentSaved(notification.SavedEntities.Count());
    }

    public void Handle(ContentPublishedNotification notification) {
        _meter.ContentPublished(notification.PublishedEntities.Count());
    }

    public void Handle(ContentUnpublishedNotification notification) {
        _meter.ContentUnpublished(notification.UnpublishedEntities.Count());
    }

    public void Handle(ContentCopiedNotification notification) {
        _meter.ContentCopied(1);
    }

    public void Handle(ContentMovedNotification notification) {
        _meter.ContentMoved(notification.MoveInfoCollection.Count());
    }

    public void Handle(ContentMovedToRecycleBinNotification notification) {
        _meter.ContentMovedToRecycleBin(notification.MoveInfoCollection.Count());
    }

    public void Handle(ContentDeletedNotification notification) {
        _meter.ContentDeleted(notification.DeletedEntities.Count());
    }

    public void Handle(ContentRolledBackNotification notification) {
        _meter.ContentRolledBack(1);
    }

    public void Handle(ContentSentToPublishNotification notification) {
        _meter.ContentSentToPublish(1);
    }

    public void Handle(ContentEmptiedRecycleBinNotification notification) {
        _meter.ContentEmptiedRecycleBin(1);
        _meter.ContentEmptiedRecycleBinCount(notification.DeletedEntities.Count());
    }

    public void Handle(ContentSavedBlueprintNotification notification) {
        _meter.ContentSavedBlueprint(1);
    }

    public void Handle(ContentDeletedBlueprintNotification notification) {
        _meter.ContentDeletedBlueprint(notification.DeletedBlueprints.Count());
    }
}
