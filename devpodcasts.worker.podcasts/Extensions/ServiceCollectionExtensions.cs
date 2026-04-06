using devpodcasts.common.Factories;
using devpodcasts.common.Interfaces;
using devpodcasts.common.Services;
using devpodcasts.common.Updaters;
using devpodcasts.Data.EntityFramework.Extensions;
using devpodcasts.Worker.Podcasts.Services;
using Serilog;

namespace devpodcasts.Worker.Podcasts.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration config)
    {
        var connString = config.GetSection("ConnectionStrings:PodcastDb").Value;

        services.AddSingleton(config);
        
        services.AddDataServices(connString);

        services.AddHttpClient<IItunesHttpClient, ItunesHttpClient>(client =>
        {
            client.BaseAddress = new Uri("https://itunes.apple.com/lookup/");
        });

        services.AddScoped<IDatabaseService, DatabaseService>();
        services.AddSingleton<IDbContextFactory, DbContextFactory>();
        services.AddScoped<IItunesPodcastUpdater, ItunesPodcastUpdater>();
        services.AddScoped<IITunesEpisodeUpdater, ItunesEpisodeUpdater>();
        services.AddScoped<IBasePodcastUpdater, BasePodcastUpdater>();
        services.AddScoped<IDataCleaner, DataCleaner>();
        services.AddSingleton<IServiceRunner, ServiceRunner>();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddSerilog();
            loggingBuilder.AddConfiguration(config.GetSection("Serilog"));
        }).Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);

        return services;
    }
}