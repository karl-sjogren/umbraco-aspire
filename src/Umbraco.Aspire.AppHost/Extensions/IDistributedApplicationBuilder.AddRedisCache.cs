namespace Umbraco.Aspire.AppHost.Extensions;

public static partial class IDistributedApplicationBuilderExtensions {
    public static IDistributedApplicationBuilder AddRedisCache(
            this IDistributedApplicationBuilder builder,
            IResourceBuilder<ProjectResource> umbracoProject,
            string redisCacheName) {
        var redisCache = builder.AddAzureRedis(redisCacheName);

        if(builder.ExecutionContext.IsRunMode) {
            redisCache
                .RunAsContainer(redis => {
                    redis.WithDataVolume();
                    redis.WithRedisInsight();
                });
        }

        redisCache.OnResourceReady(async (resource, @event, cancellationToken) => {
            await Task.Delay(1000, cancellationToken);
        });

        redisCache.OnConnectionStringAvailable(async (resource, @event, cancellationToken) => {
            await Task.Delay(1000, cancellationToken);
        });

        umbracoProject.WithReference(redisCache)
            .WaitFor(redisCache);

        return builder;
    }
}
