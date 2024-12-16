using System;
using System.IO;
using devpodcasts.common.Factories;
using devpodcasts.common.Interfaces;
using devpodcasts.data.mock;
using devpodcasts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace devpodcasts.Services.Core.Test
{
    public class DbFixture : IDisposable
    {
        public MockDbContext DbContext { get; }
        public DbFixture()
        {
            var serviceCollection  = new ServiceCollection();

            Configuration = LoadConfiguration();

            // Register the DbContextFactory using MockDbContext
            serviceCollection.AddDbContextFactory<MockDbContext>();
            serviceCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            }).Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);

            serviceCollection.AddSingleton<IPodcastGenerator, PodcastGenerator>();
            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider serviceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
        
        public void Dispose()
        {
            // Clean up resources if needed
            // For example, dispose the DbContext
            if (DbContext is IDisposable disposableDbContext)
            {
                disposableDbContext.Dispose();
            }
        }
    }
}