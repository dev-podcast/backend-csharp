using devpodcasts.Domain;
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
        var group = app.MapGroup("/v1").WithTags("Podcasts");

        group.MapGet("/podcasts", async ([FromServices] IUnitOfWork unitOfWork, string? title, DateTime? fromDate) =>
            {
                var podcasts = await unitOfWork.PodcastRepository.GetAllAsync();

                if (title != null)
                {
                    podcasts = podcasts.Where(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (fromDate != null)
                {
                    podcasts = podcasts.Where(p => p.LatestReleaseDate >= fromDate).ToList();
                }

                return Results.Ok(podcasts.ToPodcastDtos());
            })
            .WithName("GetPodcasts")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Retrieves all podcasts.";
                operation.Description = "Returns a list of podcasts, optionally filtered by title and release date.";
                return operation;
            });

        group.MapGet("/podcast/{id}", async ([FromServices] IUnitOfWork unitOfWork, Guid id) =>
        {
            var podcast = await unitOfWork.PodcastRepository.GetAsync(p => p.Id == id);

            return podcast != null ? Results.Ok(podcast.ToPodcastDto()) : Results.NotFound();
        }).WithName("GetPodcast").WithOpenApi(operation =>
        {
            operation.Summary = "Retrieves a specific podcast by ID.";
            return operation;
        });

        group.MapGet("/podcasts/recent", async ([FromServices] IUnitOfWork unitOfWork, int? podcastLimit, int? episodeLimit) =>
        {
            var pLimit = podcastLimit ?? 15;
            var eLimit = episodeLimit ?? 15;
            var recentPodcasts = await unitOfWork.PodcastRepository.GetRecentAsync(pLimit, eLimit);

            return Results.Ok(recentPodcasts.ToPodcastDtos());
        }).WithName("GetRecentPodcasts").WithOpenApi(operation =>
        {
            operation.Summary = "Retrieves recent podcasts.";
            operation.Description = "Returns a list of recent podcasts with optional limits for podcasts and episodes.";
            return operation;
        });

        group.MapGet("/podcast/tag/{id}", async ([FromServices] IUnitOfWork unitOfWork, Guid id) =>
        {
            var tag = await unitOfWork.TagRepository.GetAsync(x => x.Id == id);
            if (tag == null) return Results.NotFound();
            
            var podcasts = tag.Podcasts.ToList();
            return Results.Ok(podcasts.ToPodcastDtos());
        }).WithName("GetPodcastsByTag").WithOpenApi(operation =>
        {
            operation.Summary = "Retrieves podcasts associated with a specific tag.";
            return operation;
        });

        group.MapGet("/podcasts/search",
                async ([FromServices] IUnitOfWork unitOfWork, string? searchTerm) =>
                {
                    if (searchTerm != null)
                    {
                        var podcasts = await unitOfWork.PodcastRepository.GetAllBySearch(p =>
                            p.Title.Contains(searchTerm) || p.Description.Contains(searchTerm));

                        return Results.Ok(podcasts.ToPodcastDtos());
                    }

                    var defaultPodcasts = await unitOfWork.PodcastRepository.GetAllAsync();
                    return Results.Ok(defaultPodcasts.ToPodcastDtos());
                })
            .WithName("SearchPodcasts")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Searches for podcasts.";
                operation.Description = "Returns podcasts that match the search term in their title or description.";
                return operation;
            });

        group.MapGet("/podcasts/{id}/episodes",
                async ([FromServices] IUnitOfWork unitOfWork, Guid id) =>
                {
                    var podcast = await unitOfWork.PodcastRepository.GetAsync(p => p.Id == id);
                    if (podcast == null) return Results.NotFound();
                    
                    var episodes = await unitOfWork.EpisodeRepository.GetAllAsync(e => e.PodcastId == podcast.Id);

                    return Results.Ok(episodes.ToEpisodeDtos());
                })
            .WithName("GetPodcastEpisodes")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Retrieves all episodes for a specific podcast.";
                return operation;
            });

        group.MapGet("/podcasts/{id}/categories",
                async ([FromServices] IUnitOfWork unitOfWork, Guid id) =>
                {
                    var podcast = await unitOfWork.PodcastRepository.GetAsync(p => p.Id == id);
                    if (podcast == null) return Results.NotFound();
                    
                    return Results.Ok(podcast.Categories.ToCategoryDtos());
                })
            .WithName("GetPodcastCategories")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Retrieves categories for a specific podcast.";
                return operation;
            });
    }
}