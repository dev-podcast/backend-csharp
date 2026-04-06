using System.Reflection;
using devpodcasts.Worker.Podcasts.Extensions;
using devpodcasts.Worker.Podcasts.Services;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
    .AddUserSecrets(typeof(Program).GetTypeInfo().Assembly, optional: false).Build();


builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.AddCustomServices(configuration);
builder.Services.AddHostedService<PodcastUpdateWorker>();

var host = builder.Build();

// Ensure database is created and migrated
using (var scope = host.Services.CreateScope())
{
    var databaseService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
    if (!await databaseService.CanConnectAsync())
    {
        await databaseService.MigrateAsync();
    }
    else
    {
        var pendingMigrations = await databaseService.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await databaseService.MigrateAsync();
        }
    }
}

await host.RunAsync();