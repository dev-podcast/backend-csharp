using System;
using System.IO;
using AutoMapper;
using DevPodcast.Domain;
using DevPodcast.Data.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication
    .CreateBuilder(args);

builder.Host.UseSerilog();

var environmentSettings = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var environment = Enum.TryParse<DevPodcast.Server.Core.Enums.Environments>(environmentSettings, out var result)
    ? result
    : throw new FileLoadException(
        "ASPNETCORE_ENVIRONMENT must be set to either Prod, Qa, Test or Development.");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddMvc()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

builder.Services.AddAutoMapper();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
 {
     var connStringKey = builder.Configuration.GetSection("ConnectionStrings").GetValue<string>("PodcastDb");

     options.EnableSensitiveDataLogging(true);
     options.UseSqlServer(connStringKey, b => b.MigrationsAssembly("DevPodcast.Data.EntityFramework"));
});

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", true, true)
    .AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
