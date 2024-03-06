using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
        public static void Main(string[] args)
        {
            Task.Run(() =>
            {

                try
                {
                    var serviceCollection = ConfigureServices();
                    var serviceProvider = serviceCollection.BuildServiceProvider();

                    var serviceRunner = serviceProvider.GetService<ServiceRunner>();

                    serviceRunner.RunAsync(GetUpdaters(serviceProvider)).Wait();
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                
            }).Wait();
        }
        
        private static IConfiguration LoadConfiguration()
        {
           

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();
            
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets(typeof(Program).GetTypeInfo().Assembly, optional: false);
            
            return builder.Build();
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();
            var config = LoadConfiguration();
            
            services.AddCustomServices(config);
            var connString = config.GetSection("ConnectionStrings").GetSection("PodcastDb").Value;
            

            //services.AddHttpClient<IHttpClientFactory>();
            services.AddHttpClient<IItunesHttpClient, ItunesHttpClient>().ConfigureHttpClient(options =>
            {
                options.BaseAddress = new Uri("https://itunes.apple.com/lookup/");
            });
           
                    

            services.AddSingleton<IItunesHttpClient, ItunesHttpClient>();
           
            services.AddSingleton<IItunesPodcastUpdater, ItunesPodcastUpdater>();
            services.AddTransient<IITunesEpisodeUpdater, ItunesEpisodeUpdater>();
        //    services.AddSingleton<IBasePodcastUpdater, BasePodcastUpdater>();
        //    services.AddSingleton<IPodCategoriesUpdater, PodCategoriesUpdater>();
            services.AddSingleton<IDataCleaner, DataCleaner>();
            services.AddSingleton<IServiceRunner, ServiceRunner>();
            
            // Configure the IDbContext based on a configuration setting or environment variable
            if (config["UseMockDbContext"] == "true")
            {

                services.AddSingleton<IDbContext, MockDbContext>();
                services.AddDbContextFactory<MockDbContext>();
            }
            else
            {
                services.AddSingleton<ICategoryRepository, CategoryRepository>();
                services.AddSingleton<IBasePodcastRepository, BasePodcastRepository>();
                services.AddSingleton<IPodcastRepository, PodcastRepository>();
                services.AddSingleton<ITagRepository, TagRepository>();
                services.AddSingleton<IDbContextFactory, DbContextFactory>();   
                services.AddScoped<IUnitOfWork, UnitOfWork>();
                services.AddDbContextFactory<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(connString, op =>
                    {
                        op.MigrationsAssembly("devpodcasts.data.entityframework");
                        op.EnableRetryOnFailure();
                    }).EnableDetailedErrors();
                });
            }


            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog();
                loggingBuilder.AddConsole();
                loggingBuilder.AddConfiguration(config.GetSection("Serilog"));
            }).Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);


          
            return services;
        }
        
        private static List<IUpdater> GetUpdaters(IServiceProvider serviceProvider)
        {
            return new List<IUpdater>
            {
                serviceProvider.GetRequiredService<IPodCategoriesUpdater>(),
                serviceProvider.GetRequiredService<IBasePodcastUpdater>(),
                serviceProvider.GetRequiredService<IItunesPodcastUpdater>(),
                serviceProvider.GetRequiredService<IITunesEpisodeUpdater>(),
                serviceProvider.GetRequiredService<IDataCleaner>()
            };
        }
    }
}