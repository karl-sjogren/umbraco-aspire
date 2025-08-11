namespace Umbraco.Aspire.AppHost.Extensions;

public static partial class IDistributedApplicationBuilderExtensions {
    public static IDistributedApplicationBuilder AddRedisCache(
            this IDistributedApplicationBuilder builder,
            IResourceBuilder<ProjectResource> umbracoProject,
            string redisCacheName) {
        var redisCache = builder.AddRedis(redisCacheName)
            .WithDataVolume(isReadOnly: false);

        if(builder.ExecutionContext.IsRunMode) {
            redisCache.WithRedisInsight();
        }

        umbracoProject.WithReference(redisCache)
            .WaitFor(redisCache);

        return builder;
    }
}
