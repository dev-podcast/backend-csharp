using AutoMapper;
using devpodcasts.data.entityframework;
using devpodcasts.domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace devpodcasts.server
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
            services.AddCors();
            services.AddAutoMapper();
      
            services.AddMvc().AddJsonOptions(
                options => options.SerializerSettings.ReferenceLoopHandling
                        = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddDbContext<ApplicationDbContext>(options =>
            {             
               options.UseSqlServer(Configuration.GetConnectionString("PodcastDb"));
            });

 

            services.AddScoped<IUnitOfWork, UnitOfWork>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug(LogLevel.Trace); // ⇦ you're not passing the LogLevel!
            }

           

            loggerFactory.AddLog4Net();

            app.UseCors(builder => builder
           
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());


            

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Podcast}/{action=recent}");
            });
        }
    }
}
