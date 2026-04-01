using devpodcasts.Domain.Entities;
using devpodcasts.server.api.Models;

namespace devpodcasts.server.api.Extensions;

public static class TransformExtensions
{
    public static IEnumerable<PodcastDto> ToPodcastDtos(this IEnumerable<Podcast> podcasts)
    {
        foreach (var podcast in podcasts)
        {
            yield return podcast.ToPodcastDto();
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
            FeedUrl = podcast.FeedUrl,
            LatestReleaseDate = podcast.LatestReleaseDate,
            EpisodeCount = podcast.EpisodeCount,
            Country = podcast.Country,
            CreatedDate = podcast.CreatedDate,
            Artists = podcast.Artists,
            ItunesId = podcast.ItunesId,
            Tags = podcast.Tags != null && podcast.Tags.Any() ? podcast.Tags.Select(t => t.Description).ToList() : new List<string>(),
            Categories = podcast.Categories != null && podcast.Categories.Any() ? podcast.Categories.Select(c => c.Description).ToList() : new List<string>()
        };
    }
    
    public static IEnumerable<EpisodeDto> ToEpisodeDtos(this IEnumerable<Episode> episodes)
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
    
    public static IEnumerable<CategoryDto> ToCategoryDtos(this IEnumerable<Category> categories)
    {
        foreach (var category in categories)
        {
            yield return category.ToCategoryDto();
        }
    }

    public static CategoryDto ToCategoryDto(this Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Description = category.Description,
            Episodes = category.Episodes?.Select(e => e.Id).ToList() ?? new List<Guid>(),
            Podcasts = category.Podcasts?.Select(p => p.Id).ToList() ?? new List<Guid>()
        };
    }

    public static IEnumerable<TagDto> ToTagDtos(this IEnumerable<Tag> tags)
    {
        foreach (var tag in tags)
        {
            yield return tag.ToTagDto();
        }
    }

    public static TagDto ToTagDto(this Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Description = tag.Description,
            Episodes = tag.Episodes?.Select(e => e.Id).ToList() ?? new List<Guid>(),
            Podcasts = tag.Podcasts?.Select(p => p.Id).ToList() ?? new List<Guid>()
        };
    }

    public static SearchResultDto ToSearchResultDto(this devpodcasts.Domain.Entities.Dtos.SearchResult searchResult)
    {
        return new SearchResultDto
        {
            Episodes = searchResult.Episodes != null ? searchResult.Episodes.ToEpisodeDtos().ToList() : new List<EpisodeDto>(),
            Podcasts = searchResult.Podcasts != null ? searchResult.Podcasts.ToPodcastDtos().ToList() : new List<PodcastDto>()
        };
    }
}