using System.Globalization;
using System.Reflection;
using devpodcasts.Data.EntityFramework.Extensions;
using devpodcasts.Domain.Entities;
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteDevServer",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var _configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
    .AddUserSecrets(typeof(Program).GetTypeInfo().Assembly, optional: false).Build();
  


var connString = _configuration.GetSection("ConnectionStrings:PodcastDb").Value;

builder.Services.AddDataServices(connString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        c.RoutePrefix = string.Empty; // Keep it at root for easy access
    });
    // Add a redirect for /swagger to root
    app.MapGet("/swagger", () => Results.Redirect("/index.html"));
    app.UseCors("AllowViteDevServer");
}


app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseLoggingMiddleware();

// Conditionally use HttpsRedirection to avoid warnings in Docker/Development
if (!app.Environment.IsDevelopment() && Environment.GetEnvironmentVariable("ENABLE_SWAGGER") != "true")
{
    app.UseHttpsRedirection();
}
// app.UseExceptionHandler("/error"); // ErrorHandlingMiddleware handles exceptions




app.PodcastEndpoints();
app.EpisodeEndpoints();
app.TagEndpoints();
app.CategoryEndpoints();
app.SearchEndpoints();


app.Run();
