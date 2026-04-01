using System.Collections.Generic;
using System.Threading.Tasks;
using devpodcasts.common.Interfaces;
using devpodcasts.Services.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace devpodcasts.Services.Core.Test
{
    public class ServiceRunnerTests
    {
        [Fact]
        public async Task RunAsync_ShouldCallUpdateDataAsyncOnEachUpdater()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ServiceRunner>>();
            var serviceRunner = new ServiceRunner(loggerMock.Object);
            
            var updaterMock1 = new Mock<IUpdater>();
            var updaterMock2 = new Mock<IUpdater>();
            
            var updaters = new List<IUpdater> { updaterMock1.Object, updaterMock2.Object };

            // Act
            await serviceRunner.RunAsync(updaters);

            // Assert
            updaterMock1.Verify(u => u.UpdateDataAsync(), Times.Once);
            updaterMock2.Verify(u => u.UpdateDataAsync(), Times.Once);
        }
    }
}
