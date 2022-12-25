using AutoMapper;
using DevPodcast.Data.EntityFramework;
using DevPodcast.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DevPodcast.Server.core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.AddCors();
            services.AddAutoMapper();
            services.AddMvc()
                .AddNewtonsoftJson(
                options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);    
            

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connStringKey = Configuration.GetSection("ConnectionStrings").GetValue<string>("PodcastDb");
                var connectionString = Configuration.GetSection(connStringKey).GetValue<string>(connStringKey);
                options.EnableSensitiveDataLogging(true);
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHsts();
            }


            //loggerFactory.AddLog4Net();

            app.UseCors();

         

            app.UseHttpsRedirection();

        }
    }
}
