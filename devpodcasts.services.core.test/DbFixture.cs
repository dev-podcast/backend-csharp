using System.IO;
using DevPodcast.Data.EntityFramework;
using DevPodcast.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DevPodcast.Services.Core.Test
{
    public class DbFixture
    {
        public DbFixture()
        {
            var services = new ServiceCollection();

            Configuration = LoadConfiguration();

            services.AddTransient<IDbContextFactory, DbContextFactory>();
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            }).Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);

            ServiceProvider = services.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
    }
}