using System.Globalization;
using System.Reflection;
using devpodcasts.Data.EntityFramework;
using devpodcasts.Data.EntityFramework.Repositories;
using devpodcasts.data.mock;
using devpodcasts.Domain.Interfaces;
using devpodcasts.server.api;
using devpodcasts.server.api.Extensions;
using devpodcasts.server.api.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var _configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
    .AddUserSecrets(typeof(Program).GetTypeInfo().Assembly, optional: false).Build();
  


var connString = _configuration.GetSection("ConnectionStrings:PodcastDb").Value;

builder.Services.AddSingleton<IPodcastGenerator, PodcastGenerator>();
builder.Services.AddScoped<IPodcastRepository, PodcastRepository>();
builder.Services.AddScoped<IEpisodeRepository, EpisodeRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connString, op =>
    {
        op.MigrationsAssembly("devpodcasts.data.entityframework");
        op.EnableRetryOnFailure();
    }).EnableDetailedErrors();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseLoggingMiddleware();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseExceptionHandler("/error");




app.PodcastEndpoints();


app.Run();
