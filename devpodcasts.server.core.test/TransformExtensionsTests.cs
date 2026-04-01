using System;
using System.Collections.Generic;
using System.Linq;
using devpodcasts.Domain.Entities;
using devpodcasts.server.api.Extensions;
using devpodcasts.server.api.Models;
using Xunit;

namespace devpodcasts.Server.Core.Test
{
    public class TransformExtensionsTests
    {
        [Fact]
        public void ToPodcastDto_ShouldMapFieldsCorrectly()
        {
            // Arrange
            var podcast = new Podcast
            {
                Id = Guid.NewGuid(),
                Title = "Test Podcast",
                Description = "Test Description",
                ImageUrl = "http://test.com/image.png",
                ShowUrl = "http://test.com/show",
                FeedUrl = "http://test.com/feed",
                LatestReleaseDate = DateTime.Now,
                EpisodeCount = 10,
                Country = "US",
                CreatedDate = DateTime.Now.AddDays(-1),
                Artists = "Test Artist",
                ItunesId = "12345"
            };

            // Act
            var dto = podcast.ToPodcastDto();

            // Assert
            Assert.Equal(podcast.Id, dto.Id);
            Assert.Equal(podcast.Title, dto.Title);
            Assert.Equal(podcast.Description, dto.Description);
            Assert.Equal(podcast.ImageUrl, dto.ImageUrl);
            Assert.Equal(podcast.ShowUrl, dto.ShowUrl);
            Assert.Equal(podcast.FeedUrl, dto.FeedUrl);
            Assert.Equal(podcast.LatestReleaseDate, dto.LatestReleaseDate);
            Assert.Equal(podcast.EpisodeCount, dto.EpisodeCount);
            Assert.Equal(podcast.Country, dto.Country);
            Assert.Equal(podcast.CreatedDate, dto.CreatedDate);
            Assert.Equal(podcast.Artists, dto.Artists);
            Assert.Equal(podcast.ItunesId, dto.ItunesId);
        }

        [Fact]
        public void ToPodcastDtos_ShouldMapCollection()
        {
            // Arrange
            var podcasts = new List<Podcast>
            {
                new Podcast { Id = Guid.NewGuid(), Title = "P1" },
                new Podcast { Id = Guid.NewGuid(), Title = "P2" }
            };

            // Act
            var dtos = podcasts.ToPodcastDtos().ToList();

            // Assert
            Assert.Equal(2, dtos.Count);
            Assert.Equal(podcasts[0].Id, dtos[0].Id);
            Assert.Equal(podcasts[1].Id, dtos[1].Id);
        }

        [Fact]
        public void ToEpisodeDto_ShouldMapFieldsCorrectly()
        {
            // Arrange
            var podcastId = Guid.NewGuid();
            var episode = new Episode
            {
                Id = Guid.NewGuid(),
                Title = "Test Episode",
                Author = "Test Author",
                Description = "Test Description",
                AudioUrl = "http://test.com/audio.mp3",
                AudioType = "audio/mpeg",
                AudioDuration = "00:30:00",
                PublishedDate = DateTime.Now,
                CreatedDate = DateTime.Now.AddDays(-1),
                ImageUrl = "http://test.com/episode.png",
                SourceUrl = "http://test.com/source",
                PodcastId = podcastId,
                Podcast = new Podcast { Id = podcastId, Title = "Test Podcast" },
                Tags = new List<Tag> { new Tag { Description = "Tag1" } },
                Categories = new List<Category> { new Category { Description = "Cat1" } }
            };

            // Act
            var dto = episode.ToEpisodeDto();

            // Assert
            Assert.Equal(episode.Id, dto.Id);
            Assert.Equal(episode.Title, dto.Title);
            Assert.Equal(episode.Author, dto.Author);
            Assert.Equal(episode.Description, dto.Description);
            Assert.Equal(episode.AudioUrl, dto.AudioUrl);
            Assert.Equal(episode.AudioType, dto.AudioType);
            Assert.Equal(episode.AudioDuration, dto.AudioDuration);
            Assert.Equal(episode.PublishedDate, dto.PublishedDate);
            Assert.Equal(episode.CreatedDate, dto.CreatedDate);
            Assert.Equal(episode.ImageUrl, dto.ImageUrl);
            Assert.Equal(episode.SourceUrl, dto.SourceUrl);
            Assert.Equal(episode.PodcastId, dto.PodcastId);
            Assert.Equal(episode.Podcast.Title, dto.PodcastTitle);
            Assert.Single(dto.Tags);
            Assert.Equal("Tag1", dto.Tags[0]);
            Assert.Single(dto.Categories);
            Assert.Equal("Cat1", dto.Categories[0]);
        }

        [Fact]
        public void ToEpisodeDtos_ShouldHandleNullTagsAndCategories()
        {
            // Arrange
            var podcastId = Guid.NewGuid();
            var episodes = new List<Episode>
            {
                new Episode 
                { 
                    Id = Guid.NewGuid(), 
                    Title = "E1", 
                    Podcast = new Podcast { Title = "P1" },
                    Tags = null,
                    Categories = null
                }
            };

            // Act
            var dtos = episodes.ToEpisodeDtos().ToList();

            // Assert
            Assert.Empty(dtos[0].Tags);
            Assert.Empty(dtos[0].Categories);
        }

        [Fact]
        public void ToTagDto_ShouldMapFieldsCorrectly()
        {
            // Arrange
            var tag = new Tag
            {
                Id = Guid.NewGuid(),
                Description = "Test Tag",
                Episodes = new List<Episode> { new Episode { Id = Guid.NewGuid() } },
                Podcasts = new List<Podcast> { new Podcast { Id = Guid.NewGuid() } }
            };

            // Act
            var dto = tag.ToTagDto();

            // Assert
            Assert.Equal(tag.Id, dto.Id);
            Assert.Equal(tag.Description, dto.Description);
            Assert.Single(dto.Episodes);
            Assert.Single(dto.Podcasts);
        }

        [Fact]
        public void ToCategoryDto_ShouldMapFieldsCorrectly()
        {
            // Arrange
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Description = "Test Category",
                Episodes = new List<Episode> { new Episode { Id = Guid.NewGuid() } },
                Podcasts = new List<Podcast> { new Podcast { Id = Guid.NewGuid() } }
            };

            // Act
            var dto = category.ToCategoryDto();

            // Assert
            Assert.Equal(category.Id, dto.Id);
            Assert.Equal(category.Description, dto.Description);
            Assert.Single(dto.Episodes);
            Assert.Single(dto.Podcasts);
        }
    }
}
