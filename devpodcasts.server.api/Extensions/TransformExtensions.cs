using devpodcasts.Domain.Entities;
using devpodcasts.server.api.Models;

namespace devpodcasts.server.api.Extensions;

public static class TransformExtensions
{
    public static IEnumerable<PodcastDto> ToPodcastDtos(this List<Podcast> podcasts)
    {
        foreach (var podcast in podcasts)
        {
            yield return new PodcastDto
            {
                Id = podcast.Id,
                Title = podcast.Title,
                Description = podcast.Description,
                ImageUrl = podcast.ImageUrl,
                ShowUrl = podcast.ShowUrl,
                LatestReleaseDate = podcast.LatestReleaseDate
            };
        };
    }

    public static PodcastDto ToPodcastDto(this Podcast podcast)
    {
        return new PodcastDto
        {
            Id = podcast.Id,
            Title = podcast.Title,
            Description = podcast.Description,
            ImageUrl = podcast.ImageUrl,
            ShowUrl = podcast.ShowUrl,
            LatestReleaseDate = podcast.LatestReleaseDate
        };
    }
    
    public static IEnumerable<EpisodeDto> ToEpisodeDtos(this List<Episode> episodes)
    {
        foreach (var episode in episodes)
        {
            yield return new EpisodeDto
            {
                Id = episode.Id,
                Title = episode.Title,
                Description = episode.Description,
                ImageUrl = episode.ImageUrl,
                AudioUrl = episode.AudioUrl,
                AudioType = episode.AudioType,
                AudioDuration = episode.AudioDuration,
                PublishedDate = episode.PublishedDate,
                CreatedDate = episode.CreatedDate,
                SourceUrl = episode.SourceUrl,
                PodcastId = episode.PodcastId,
                PodcastTitle = episode.Podcast.Title,
                Tags = episode.Tags != null && episode.Tags.Any()
                    ? episode.Tags.Select(t => t.Description).ToList()
                    : new List<string>(),
                Categories = episode.Categories != null && episode.Categories.Any()
                    ? episode.Categories.Select(c => c.Description).ToList()
                    : new List<string>()
            };
        }
    }

    public static EpisodeDto ToEpisodeDto(this Episode episode)
    {
        return new EpisodeDto
        {
            Id = episode.Id,
            Title = episode.Title,
            Author = episode.Author,
            Description = episode.Description,
            AudioUrl = episode.AudioUrl,
            AudioType = episode.AudioType,
            AudioDuration = episode.AudioDuration,
            PublishedDate = episode.PublishedDate,
            CreatedDate = episode.CreatedDate,
            ImageUrl = episode.ImageUrl,
            SourceUrl = episode.SourceUrl,
            PodcastId = episode.PodcastId,
            PodcastTitle = episode.Podcast.Title,
            Tags = episode.Tags != null && episode.Tags.Any() ? episode.Tags.Select(t => t.Description).ToList() : new List<string>(),
            Categories = episode.Categories != null && episode.Categories.Any() ?  episode.Categories.Select(c => c.Description).ToList() : new List<string>()
        };
    }
    
    public static IEnumerable<CategoryDto> ToCategoryDtos(this ICollection<Category> categories)
    {
        foreach (var category in categories)
        {
            yield return new CategoryDto
            {
                Id = category.Id,
                Description = category.Description,
                Episodes = category.Episodes.Any() ? category.Episodes.Select(e => e.Id).ToList() : new List<Guid>(),
                Podcasts = category.Podcasts.Any() ? category.Podcasts.Select(p => p.Id).ToList() : new List<Guid>()
            };
        }
    }
}