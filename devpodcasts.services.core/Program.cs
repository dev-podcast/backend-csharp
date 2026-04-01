using devpodcasts.common.Interfaces;
using devpodcasts.Services.Core.Extensions;
using devpodcasts.Services.Core.Services;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.AddCustomServices(builder.Configuration);
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