using devpodcasts.common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace devpodcasts.common.Extensions;

public static class CommonHttpServiceCollectionExtensions
{
    public static void AddCommonHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<CommonHttpClient>();
    }
}