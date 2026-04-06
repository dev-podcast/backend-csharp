using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using devpodcasts.common.Interfaces;
using devpodcasts.common.Services;
using devpodcasts.common.Updaters;
using devpodcasts.Domain;
using devpodcasts.Domain.Entities;
using devpodcasts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace devpodcasts.Worker.Podcasts.Test;

public class ItunesEpisodeUpdaterTests
{
    private readonly Mock<ILogger<ItunesEpisodeUpdater>> _loggerMock;
    private readonly Mock<IItunesHttpClient> _itunesHttpClientMock;
    private readonly Mock<IEpisodeRepository> _episodeRepositoryMock;
    private readonly Mock<IPodcastRepository> _podcastRepositoryMock;
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public ItunesEpisodeUpdaterTests()
    {
        _loggerMock = new Mock<ILogger<ItunesEpisodeUpdater>>();
        _itunesHttpClientMock = new Mock<IItunesHttpClient>();
        _episodeRepositoryMock = new Mock<IEpisodeRepository>();
        _podcastRepositoryMock = new Mock<IPodcastRepository>();
        _tagRepositoryMock = new Mock<ITagRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _unitOfWorkMock.Setup(x => x.CreateExecutionStrategy()).Returns(new MockExecutionStrategy());
        _unitOfWorkMock.Setup(x => x.PodcastRepository).Returns(_podcastRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.EpisodeRepository).Returns(_episodeRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.TagRepository).Returns(_tagRepositoryMock.Object);
    }

    private class MockExecutionStrategy : IExecutionStrategy
    {
        public bool RetriesOnFailure => false;
        public TResult Execute<TState, TResult>(TState state, Func<DbContext, TState, TResult> operation, Func<DbContext, TState, ExecutionResult<TResult>>? verifySucceeded) => throw new NotImplementedException();
        public Task<TResult> ExecuteAsync<TState, TResult>(TState state, Func<DbContext, TState, CancellationToken, Task<TResult>> operation, Func<DbContext, TState, CancellationToken, Task<ExecutionResult<TResult>>>? verifySucceeded, CancellationToken cancellationToken = new CancellationToken()) => operation(null!, state, cancellationToken);
        public Task ExecuteAsync(Func<Task> operation, Func<Task<bool>>? verifySucceeded, CancellationToken cancellationToken = new CancellationToken()) => operation();
    }

    [Fact]
    public async Task UpdateDataAsync_ShouldProcessPodcastsAndEpisodes()
    {
        // Arrange
        var podcastId = Guid.NewGuid();
        var podcast = new Podcast { Id = podcastId, Title = "Test Podcast", FeedUrl = "http://test.com/feed", Tags = new List<Tag>() };
        var podcasts = new List<Podcast> { podcast };

        _podcastRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(podcasts);
        
        XNamespace itunesNs = "http://www.itunes.com/dtds/podcast-1.0.dtd";
        var xmlItem = new XElement("item",
            new XElement("title", "Test Episode"),
            new XElement("enclosure", new XAttribute("url", "http://test.com/audio.mp3"), new XAttribute("type", "audio/mpeg")),
            new XElement("pubDate", DateTime.Now.ToString("R")),
            new XElement(itunesNs + "summary", "Summary")
        );
        var feedItems = new List<XElement> { xmlItem };

        _itunesHttpClientMock.Setup(x => x.QueryFeedUrl(podcast.FeedUrl)).ReturnsAsync(feedItems);
        _episodeRepositoryMock.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Episode, bool>>>())).ReturnsAsync((Episode)null);
        _episodeRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Episode, bool>>>())).ReturnsAsync(new List<Episode>());
        _tagRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tag, bool>>>())).ReturnsAsync(new List<Tag>());

        var updater = new ItunesEpisodeUpdater(
            _loggerMock.Object,
            _itunesHttpClientMock.Object,
            _episodeRepositoryMock.Object,
            _podcastRepositoryMock.Object,
            _tagRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        await updater.UpdateDataAsync();

        // Assert
        _itunesHttpClientMock.Verify(x => x.QueryFeedUrl(podcast.FeedUrl), Times.Once);
        // AddRangeAsync is used in SaveTagsAndEpisodeTags which is called from UpdateDataAsync
        _episodeRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<IEnumerable<Episode>>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task UpdateDataAsync_ShouldNotAddDuplicateEpisode()
    {
        // Arrange
        var podcastId = Guid.NewGuid();
        var podcast = new Podcast { Id = podcastId, Title = "Test Podcast", FeedUrl = "http://test.com/feed", Tags = new List<Tag>() };
        var podcasts = new List<Podcast> { podcast };

        _podcastRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(podcasts);
        
        var xmlItem = new XElement("item",
            new XElement("title", "Existing Episode"),
            new XElement("enclosure", new XAttribute("url", "http://test.com/audio.mp3"), new XAttribute("type", "audio/mpeg"))
        );
        var feedItems = new List<XElement> { xmlItem };

        _itunesHttpClientMock.Setup(x => x.QueryFeedUrl(podcast.FeedUrl)).ReturnsAsync(feedItems);
        
        // Mock existing episode
        var existingEpisode = new Episode { Title = "Existing Episode", PodcastId = podcastId };
        _episodeRepositoryMock.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Episode, bool>>>())).ReturnsAsync(existingEpisode);
        _episodeRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Episode, bool>>>())).ReturnsAsync(new List<Episode> { existingEpisode });
        _tagRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tag, bool>>>())).ReturnsAsync(new List<Tag>());

        var updater = new ItunesEpisodeUpdater(
            _loggerMock.Object,
            _itunesHttpClientMock.Object,
            _episodeRepositoryMock.Object,
            _podcastRepositoryMock.Object,
            _tagRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        await updater.UpdateDataAsync();

        // Assert
        _episodeRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<IEnumerable<Episode>>()), Times.Never);
    }
}
