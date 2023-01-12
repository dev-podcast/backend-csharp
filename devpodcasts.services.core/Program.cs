﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DevPodcast.Data.EntityFramework;
using DevPodcast.Domain;
using DevPodcast.Services.Core.Interfaces;
using DevPodcast.Services.Core.Updaters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DevPodcast.Services.Core
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Task.Run(() =>
            {               
                var serviceCollection = ConfigureServices();
                var provider = serviceCollection.BuildServiceProvider();           
                var podCategoriesUpdater = provider.GetRequiredService<IPodCategoriesUpdater>();
                var basePodcastUpdater = provider.GetRequiredService<IBasePodcastUpdater>();
                var itunesPodcastUpdater = provider.GetRequiredService<IItunesPodcastUpdater>();
                var itunesEpisodeUpdater = provider.GetRequiredService<IITunesEpisodeUpdater>();
                var dataCleaner = provider.GetRequiredService<IDataCleaner>();

                var serviceRunner = provider.GetService<IServiceRunner>();

                serviceRunner.RunAsync(new List<IUpdater>
                {
                    podCategoriesUpdater,
                    basePodcastUpdater,
                    itunesPodcastUpdater,
                    itunesEpisodeUpdater,
                  //  dataCleaner
                }).Wait();
            }).Wait();
        }


        public static IConfiguration LoadConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true);

            var configuration = builder.Build();

            return configuration;
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();
            var config = LoadConfiguration();

            services.AddSingleton(config);
            services.AddTransient<IDbContextFactory, DbContextFactory>();
            services.AddSingleton<IItunesQueryService, ItunesQueryService>();
            services.AddTransient<IItunesPodcastUpdater, ItunesPodcastUpdater>();
            services.AddTransient<IITunesEpisodeUpdater, ItunesEpisodeUpdater>();
            services.AddTransient<IBasePodcastUpdater, BasePodcastUpdater>();
            services.AddTransient<IPodCategoriesUpdater, PodCategoriesUpdater>();
            services.AddTransient<IDataCleaner, DataCleaner>();
            services.AddTransient<IServiceRunner, ServiceRunner>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog();
                loggingBuilder.AddConfiguration(config.GetSection("Serilog"));
            }).Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);

            return services;
        }
    }
}