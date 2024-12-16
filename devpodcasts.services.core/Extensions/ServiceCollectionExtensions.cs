using System;
using devpodcasts.common.Factories;
using devpodcasts.common.Interfaces;
using devpodcasts.common.Services;
using devpodcasts.common.Updaters;
using devpodcasts.Data.EntityFramework;
using devpodcasts.Data.EntityFramework.Repositories;
using devpodcasts.Domain;
using devpodcasts.Domain.Interfaces;
using devpodcasts.Services.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace devpodcasts.Services.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration config)
    {
        var connString = config.GetConnectionString("PodcastDb");

        services.AddSingleton(config);
        services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connString, op =>
            {
                op.MigrationsAssembly("devpodcasts.data.entityframework");
                op.EnableRetryOnFailure();
            }).EnableDetailedErrors();
        });

        services.AddHttpClient<IItunesHttpClient, ItunesHttpClient>(client =>
        {
            client.BaseAddress = new Uri("https://itunes.apple.com/lookup/");
        });

        services.AddSingleton<IDbContextFactory, DbContextFactory>();
        services.AddSingleton<IItunesPodcastUpdater, ItunesPodcastUpdater>();
        services.AddTransient<IITunesEpisodeUpdater, ItunesEpisodeUpdater>();
        services.AddSingleton<ICategoryRepository, CategoryRepository>();
        services.AddSingleton<IBasePodcastRepository, BasePodcastRepository>();
        services.AddSingleton<IPodcastRepository, PodcastRepository>();
        services.AddSingleton<ITagRepository, TagRepository>();
        services.AddSingleton<IDataCleaner, DataCleaner>();
        services.AddSingleton<IServiceRunner, ServiceRunner>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddSerilog();
            loggingBuilder.AddConfiguration(config.GetSection("Serilog"));
        }).Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);

        return services;
    }
}