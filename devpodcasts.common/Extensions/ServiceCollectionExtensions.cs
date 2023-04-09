using devpodcasts.common.Updaters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devpodcasts.common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUpdaters(this IServiceCollection services)
        {
            services.AddTransient<IItunesPodcastUpdater, ItunesPodcastUpdater>();
            services.AddTransient<IITunesEpisodeUpdater, ItunesEpisodeUpdater>();
            services.AddTransient<IBasePodcastUpdater, BasePodcastUpdater>();
            services.AddTransient<IPodCategoriesUpdater, PodCategoriesUpdater>();
            return services;
        }
    }
}
