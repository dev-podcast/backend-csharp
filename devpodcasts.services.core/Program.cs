using devpodcasts.Data.EntityFramework;
using devpodcasts.Domain;
using devpodcasts.common.Factories;
using devpodcasts.common.Interfaces;
using devpodcasts.common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;
using devpodcasts.common.Updaters;
using Microsoft.EntityFrameworkCore;
using devpodcasts.Domain.Interfaces;
using devpodcasts.Data.EntityFramework.Repositories;
using devpodcasts.data.mock;
using devpodcasts.Services.Core.Extensions;
using devpodcasts.Services.Core.Services;

namespace devpodcasts.Services.Core
{
    internal class Program
    {
        private static IConfiguration _configuration;

        public static void Main(string[] args)
        {
            _configuration = LoadConfiguration();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();


            try
            {
                Log.Information("Starting application");

                var serviceCollection = ConfigureServices();
                var serviceProvider = serviceCollection.BuildServiceProvider();

                Task.Run(async () => await RunApplication(serviceProvider)).Wait();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IConfiguration LoadConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .AddUserSecrets(typeof(Program).GetTypeInfo().Assembly, optional: false).Build();
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();
            
            services.AddCustomServices(_configuration);
            var connString = _configuration.GetSection("ConnectionStrings:PodcastDb").Value;
            // Configure the IDbContext based on a configuration setting or environment variable
            var useMockDb = _configuration["UseMockDbContext"];

            if (useMockDb == "true")
            {
                services.AddSingleton<IDbContext, MockDbContext>();
                services.AddDbContextFactory<MockDbContext>();
            }
            else
            {
                services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
                services.AddScoped<ICategoryRepository, CategoryRepository>();
                services.AddScoped<IBasePodcastRepository, BasePodcastRepository>();
                services.AddScoped<IEpisodeRepository, EpisodeRepository>();
                services.AddScoped<IPodcastRepository, PodcastRepository>();
                services.AddScoped<ITagRepository, TagRepository>();
                services.AddScoped<IDbContextFactory, DbContextFactory>();
                services.AddScoped<IUnitOfWork, UnitOfWork>();

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(connString, op =>
                    {
                        op.MigrationsAssembly("devpodcasts.data.entityframework");
                        op.EnableRetryOnFailure();
                    }).EnableDetailedErrors();
                });
            }

            services.AddHttpClient<IItunesHttpClient, ItunesHttpClient>().ConfigureHttpClient(options =>
            {
                options.BaseAddress = new Uri("https://itunes.apple.com/lookup/");
            });


            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<IItunesHttpClient, ItunesHttpClient>();
            services.AddSingleton<IItunesPodcastUpdater, ItunesPodcastUpdater>();
            services.AddTransient<IITunesEpisodeUpdater, EpisodeUpdater>();
            services.AddScoped<IBasePodcastUpdater, BasePodcastUpdater>();
            services.AddSingleton<IPodCategoriesUpdater, PodCategoriesUpdater>();
            services.AddSingleton<IDataCleaner, DataCleaner>();
            services.AddSingleton<IServiceRunner, ServiceRunner>();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });

            return services;
        }

        private static List<IUpdater> GetUpdaters(IServiceProvider serviceProvider)
        {
            return
            [
                serviceProvider.GetRequiredService<IBasePodcastUpdater>(),
                // serviceProvider.GetRequiredService<IPodCategoriesUpdater>(),
                serviceProvider.GetRequiredService<IItunesPodcastUpdater>(),
                serviceProvider.GetRequiredService<IITunesEpisodeUpdater>(),
                serviceProvider.GetRequiredService<IDataCleaner>()
            ];
        }

        private static async Task RunApplication(IServiceProvider serviceProvider)
        {
            try
            {
                var databaseService = serviceProvider.GetRequiredService<IDatabaseService>();

                Console.WriteLine("Checking if the database exists...");
                if (!await databaseService.CanConnectAsync())
                {
                    Console.WriteLine("Database does not exist. Creating database...");
                    await databaseService.MigrateAsync();
                    Console.WriteLine("Database created and migrations applied successfully.");
                }
                else
                {
                    Console.WriteLine("Database exists.");
                    var pendingMigrations = await databaseService.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        Console.WriteLine("Applying pending migrations...");
                        await databaseService.MigrateAsync();
                        Console.WriteLine("Migrations applied successfully.");
                    }
                    else
                    {
                        Console.WriteLine("No pending migrations.");
                    }
                }

                var serviceRunner = serviceProvider.GetRequiredService<IServiceRunner>();
                var updaters = GetUpdaters(serviceProvider);
                await serviceRunner.RunAsync(updaters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred during application execution");
                throw;
            }
        }
    }
}