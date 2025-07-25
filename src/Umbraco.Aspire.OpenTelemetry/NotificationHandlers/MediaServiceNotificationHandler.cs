using Microsoft.Extensions.DependencyInjection;
using Umbraco.Aspire.OpenTelemetry.Meters;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Umbraco.Aspire.OpenTelemetry.NotificationHandlers;

public class MediaServiceNotificationHandler :
        INotificationHandler<MediaSavedNotification>,
        INotificationHandler<MediaMovedNotification>,
        INotificationHandler<MediaMovedToRecycleBinNotification>,
        INotificationHandler<MediaDeletedNotification> {
    private readonly MediaServiceMeter _meter;

    public MediaServiceNotificationHandler(MediaServiceMeter meter) {
        _meter = meter;
    }

    internal static void RegisterNotificationHandlers(IUmbracoBuilder builder) {
        builder.Services.AddSingleton<MediaServiceMeter>();

        builder.AddNotificationHandler<MediaSavedNotification, MediaServiceNotificationHandler>();
        builder.AddNotificationHandler<MediaMovedNotification, MediaServiceNotificationHandler>();
        builder.AddNotificationHandler<MediaMovedToRecycleBinNotification, MediaServiceNotificationHandler>();
        builder.AddNotificationHandler<MediaDeletedNotification, MediaServiceNotificationHandler>();
    }

    public void Handle(MediaSavedNotification notification) {
        _meter.MediaSaved(notification.SavedEntities.Count());
    }

    public void Handle(MediaMovedNotification notification) {
        _meter.MediaMoved(notification.MoveInfoCollection.Count());
    }

    public void Handle(MediaMovedToRecycleBinNotification notification) {
        _meter.MediaMovedToRecycleBin(notification.MoveInfoCollection.Count());
    }

    public void Handle(MediaDeletedNotification notification) {
        _meter.MediaDeleted(notification.DeletedEntities.Count());
    }
}
