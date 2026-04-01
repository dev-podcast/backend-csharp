using devpodcasts.Domain;
using devpodcasts.Domain.Interfaces;
using devpodcasts.server.api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace devpodcasts.server.api;

public static class EpisodeExtensions
{
    public static void EpisodeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/v1/episodes").WithTags("Episodes");

        group.MapGet("/{showId}/{episodeId}", async ([FromServices] IUnitOfWork unitOfWork, Guid showId, Guid episodeId) =>
        {
            var episode = await unitOfWork.EpisodeRepository.GetAsync(x => x.PodcastId == showId && x.Id == episodeId);
            return episode != null ? Results.Ok(episode.ToEpisodeDto()) : Results.NotFound();
        }).WithName("GetEpisode").WithOpenApi(operation =>
        {
            operation.Summary = "Retrieves a specific episode by show ID and episode ID.";
            return operation;
        });

        group.MapGet("/all/{showId}", async ([FromServices] IUnitOfWork unitOfWork, Guid showId) =>
        {
            var episodes = await unitOfWork.EpisodeRepository.GetByShowIdAsync(showId);
            return Results.Ok(episodes.ToEpisodeDtos());
        }).WithName("GetAllEpisodes").WithOpenApi(operation =>
        {
            operation.Summary = "Retrieves all episodes for a specific show.";
            return operation;
        });

        group.MapGet("/recent/{showId}", async ([FromServices] IUnitOfWork unitOfWork, Guid showId, int? limit) =>
        {
            var numberToTake = limit ?? 15;
            var episodes = await unitOfWork.EpisodeRepository.GetRecentAsync(showId, numberToTake);
            return Results.Ok(episodes.ToEpisodeDtos());
        }).WithName("GetRecentEpisodes").WithOpenApi(operation =>
        {
            operation.Summary = "Retrieves recent episodes for a specific show.";
            return operation;
        });
    }
}
