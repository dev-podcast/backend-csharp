// using Microsoft.AspNetCore;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.Extensions.Configuration;
//
// namespace DevPodcast.Server.Core
// {
//     public class Program
//     {
//         public static void Main(string[] args)  
//         {
//             CreateWebHostBuilder(args).Build().Run();
//         }
//
//         public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
//             WebHost.CreateDefaultBuilder(args)
//                 .ConfigureAppConfiguration((hostingContext, config) =>
//                 {
//                     config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
//                         optional: true, reloadOnChange: true);
//                 })
//                 .UseStartup<Startup>();
//     }
// }

using System;
using System.Configuration;
using System.IO;
using AutoMapper;
using DevPodcast.Domain;
using DevPodcast.Data.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

var builder = WebApplication
    .CreateBuilder(args);

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
     var connectionString = builder.Configuration.GetSection(connStringKey).GetValue<string>(connStringKey);
     options.EnableSensitiveDataLogging(true);
     options.UseSqlServer(connStringKey);
});

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", true, true)
    //.AddUserSecrets<Program>(true, true)
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
