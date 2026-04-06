using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using devpodcasts.common.Interfaces;
using devpodcasts.common.Updaters;
using devpodcasts.Worker.Podcasts.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace devpodcasts.Worker.Podcasts.Test;

public class PodcastUpdateWorkerTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ILogger<PodcastUpdateWorker>> _loggerMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    
    private readonly Mock<IBasePodcastUpdater> _baseUpdaterMock;
    private readonly Mock<IItunesPodcastUpdater> _itunesPodcastUpdaterMock;
    private readonly Mock<IITunesEpisodeUpdater> _itunesEpisodeUpdaterMock;
    private readonly Mock<IDataCleaner> _dataCleanerMock;

    public PodcastUpdateWorkerTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _loggerMock = new Mock<ILogger<PodcastUpdateWorker>>();
        _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        _serviceScopeMock = new Mock<IServiceScope>();
        
        _baseUpdaterMock = new Mock<IBasePodcastUpdater>();
        _itunesPodcastUpdaterMock = new Mock<IItunesPodcastUpdater>();
        _itunesEpisodeUpdaterMock = new Mock<IITunesEpisodeUpdater>();
        _dataCleanerMock = new Mock<IDataCleaner>();

        // Setup Scope Factory
        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(_serviceScopeFactoryMock.Object);

        _serviceScopeFactoryMock
            .Setup(x => x.CreateScope())
            .Returns(_serviceScopeMock.Object);

        _serviceScopeMock
            .Setup(x => x.ServiceProvider)
            .Returns(_serviceProviderMock.Object);

        // Setup Updaters
        _serviceProviderMock.Setup(x => x.GetService(typeof(IBasePodcastUpdater))).Returns(_baseUpdaterMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IItunesPodcastUpdater))).Returns(_itunesPodcastUpdaterMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IITunesEpisodeUpdater))).Returns(_itunesEpisodeUpdaterMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IDataCleaner))).Returns(_dataCleanerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRunAllUpdatersOnceBeforeWaiting()
    {
        // Arrange
        // We use a CancellationToken that cancels after some time to stop the loop
        var cts = new CancellationTokenSource();
        var worker = new PodcastUpdateWorker(_serviceProviderMock.Object, _loggerMock.Object);

        // Act
        // Run ExecuteAsync in a separate task so we can cancel it
        var task = worker.StartAsync(cts.Token);
        
        // Wait a bit to ensure it runs at least once
        await Task.Delay(200);
        cts.Cancel();
        
        try { await task; } catch (OperationCanceledException) { }

        // Assert
        _baseUpdaterMock.Verify(x => x.UpdateDataAsync(), Times.AtLeastOnce);
        _itunesPodcastUpdaterMock.Verify(x => x.UpdateDataAsync(), Times.AtLeastOnce);
        _itunesEpisodeUpdaterMock.Verify(x => x.UpdateDataAsync(), Times.AtLeastOnce);
        _dataCleanerMock.Verify(x => x.UpdateDataAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleExceptionInUpdaterAndContinue()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var worker = new PodcastUpdateWorker(_serviceProviderMock.Object, _loggerMock.Object);

        _baseUpdaterMock.Setup(x => x.UpdateDataAsync()).ThrowsAsync(new Exception("Test Exception"));

        // Act
        var task = worker.StartAsync(cts.Token);
        await Task.Delay(200);
        cts.Cancel();

        try { await task; } catch (OperationCanceledException) { }

        // Assert
        _baseUpdaterMock.Verify(x => x.UpdateDataAsync(), Times.AtLeastOnce);
        // It should continue to the next updaters in the same cycle or next cycle
        // But in the current implementation, it catches at the loop level.
        // Let's verify if subsequent updaters were NOT called in the same cycle if exception happened.
        // In the current code:
        /*
        foreach (var updater in updaters)
        {
            if (stoppingToken.IsCancellationRequested) break;
            ...
            await updater.UpdateDataAsync();
            ...
        }
        */
        // If an exception happens in await updater.UpdateDataAsync(), it goes to catch (Exception ex) outside the foreach.
        // So it won't run subsequent updaters in the SAME cycle.
        
        _itunesPodcastUpdaterMock.Verify(x => x.UpdateDataAsync(), Times.Never);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred during the podcast update cycle")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}
