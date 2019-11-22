using System;
using DevPodcast.Data.EntityFramework;
using DevPodcast.Domain;
using DevPodcast.Services.Core.Interfaces;
using DevPodcast.Services.Core.Updaters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

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

                var podCategoriesUpdater = provider.GetRequiredService<PodCategoriesUpdater>();
                var basePodcastUpdater = provider.GetRequiredService<BasePodcastUpdater>();
                var itunesPodcastUpdater = provider.GetRequiredService<ItunesPodcastUpdater>();
                var itunesEpisodeUpdater = provider.GetRequiredService<ItunesEpisodeUpdater>();
                var dataCleaner = provider.GetRequiredService<DataCleaner>();

                var consoleApp = provider.GetService<ServiceRunner>();

                consoleApp.Run(new List<IUpdater>()
                {
                    podCategoriesUpdater,
                    basePodcastUpdater,
                    itunesPodcastUpdater,
                    itunesEpisodeUpdater,
                    dataCleaner
                }).Wait();
            }).Wait();
        }

     
        public static IConfiguration LoadConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional:true, reloadOnChange: true)
                .AddSecretsManager();
            return builder.Build();
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();
            var config = LoadConfiguration();

            services.AddSingleton(config);

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IDbContextFactory, DbContextFactory>();
            services.AddTransient<ServiceRunner>();
            services.AddTransient<PodCategoriesUpdater>();
            services.AddTransient<BasePodcastUpdater>();
            services.AddTransient<ItunesPodcastUpdater>();
            services.AddTransient<ItunesEpisodeUpdater>();
            services.AddTransient<DataCleaner>();


            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(config.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            }).Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);

            return services;
        }
    }
}
