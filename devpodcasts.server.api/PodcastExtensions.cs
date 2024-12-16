using devpodcasts.Domain.Interfaces;
using devpodcasts.server.api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace devpodcasts.server.api;

public static class PodcastExtensions
{
    /// <summary>
    /// Specifies the endpoints related to podcasts in the API.
    /// </summary>
    /// <param name="app">The WebApplication instance to which the podcast endpoints will be added.</param>
    public static void PodcastEndpoints(this WebApplication app)
    {
        /// <summary>
        /// Retrieves all podcasts.
        /// </summary>
        /// <returns>A list of podcasts.</returns>
        app.MapGet("/v1/podcasts", async ([FromServices] IPodcastRepository podcastRepository, string? title, DateTime? fromDate) =>
            {
                var podcasts = await podcastRepository.GetAllAsync();

                if (title != null)
                {
                    podcasts = podcasts.Where(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (fromDate != null)
                {
                    podcasts = podcasts.Where(p => p.LatestReleaseDate >= fromDate).ToList();
                }


                return Results.Ok(podcasts.ToPodcastDtos().ToList());
            })
            .WithName("GetPodcasts")
            .WithTags("Podcasts")
            .WithOpenApi();
        
      
        app.MapGet("v1/podcast/{id}", async ([FromServices] IPodcastRepository podcastRepository, Guid id) =>
        {
            var podcast = await podcastRepository.GetAsync(p => p.Id == id);
            
            return podcast != null ? Results.Ok(podcast.ToPodcastDto()) : Results.NotFound();
        }).WithName("GetPodcast").WithTags("Podcasts").WithOpenApi();
        
        
        app.MapGet("v1/podcasts/recent", async ([FromServices] IPodcastRepository podcastRepository) =>
        {
            var recentPodcasts = await podcastRepository.GetRecentAsync(50, 50);

            return Results.Ok(recentPodcasts.ToPodcastDtos().ToList());
        }).WithName("GetRecentPodcasts").WithTags("Podcasts").WithOpenApi();
        
        app.MapGet("v1/podcasts/search",
                async ([FromServices] IPodcastRepository podcastRepository, string? searchTerm) =>
                {
                    if (searchTerm != null)
                    {
                        var podcasts = await podcastRepository.GetAllBySearch(p =>
                            p.Title.Contains(searchTerm) || p.Description.Contains(searchTerm));

                        return Results.Ok(podcasts.ToPodcastDtos());
                    }

                    var defaultPodcasts = await podcastRepository.GetAllAsync();
                    return Results.Ok(defaultPodcasts.ToPodcastDtos());
                })
            .WithName("SearchPodcasts")
            .WithTags("Podcasts")
            .WithOpenApi();
        
        app.MapGet("v1/podcasts/{id}/episodes",
                async ([FromServices] IPodcastRepository podcastRepository, [FromServices] IEpisodeRepository episodeRepository,  Guid id) =>
                {
                    var podcast = await podcastRepository.GetAsync(p => p.Id == id);
                    var episodes = await episodeRepository.GetAllAsync(e => e.PodcastId == podcast.Id);
                    
                    return Results.Ok(episodes.ToEpisodeDtos().ToList());
                })
            .WithName("GetPodcastEpisodes")
            .WithTags("Podcasts")
            .WithOpenApi();
        
        app.MapGet("v1/podcasts/{id}/categories",
                async ([FromServices] IPodcastRepository podcastRepository, [FromServices] ICategoryRepository categoryRepository, Guid id) =>
                {
                    var podcast = await podcastRepository.GetAsync(p => p.Id == id);
                    return Results.Ok(podcast.Categories.ToCategoryDtos());
                })
            .WithName("GetPodcastCategories")
            .WithTags("Podcasts")
            .WithOpenApi();
    }
}