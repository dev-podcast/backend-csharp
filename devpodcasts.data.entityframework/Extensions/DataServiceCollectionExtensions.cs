using devpodcasts.Data.EntityFramework.Repositories;
using devpodcasts.Domain;
using devpodcasts.Domain.Entities;
using devpodcasts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace devpodcasts.Data.EntityFramework.Extensions;

public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, op =>
            {
                op.MigrationsAssembly("devpodcasts.data.entityframework");
                op.EnableRetryOnFailure();
            }).EnableDetailedErrors();
        });

        services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, op =>
            {
                op.MigrationsAssembly("devpodcasts.data.entityframework");
                op.EnableRetryOnFailure();
            }).EnableDetailedErrors();
        });

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBasePodcastRepository, BasePodcastRepository>();
        services.AddScoped<IPodcastRepository, PodcastRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IEpisodeRepository, EpisodeRepository>();
        services.AddScoped<ISearchRepository, SearchRepository>();
        
        services.AddScoped<IRepository<BasePodcast>, Repository<BasePodcast>>();
        services.AddScoped<IRepository<Podcast>, Repository<Podcast>>();
        services.AddScoped<IRepository<Episode>, Repository<Episode>>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
