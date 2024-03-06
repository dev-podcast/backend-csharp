using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using devpodcasts.common.Interfaces;
using Microsoft.Extensions.Configuration;
using devpodcasts.common.Updaters;
using devpodcasts.Data.EntityFramework;
using devpodcasts.data.mock;
using devpodcasts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace devpodcasts.Services.Core.Test
{
    public class BasePodcastUpdaterTests : IClassFixture<DbFixture>
    {
        private readonly IDbContext _dbContext;
        private readonly BasePodcastUpdater _updater;
        private readonly IDbContextFactory<MockDbContext> _contextFactory;

        public BasePodcastUpdaterTests(DbFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _updater = new BasePodcastUpdater(new Mock<ILogger<BasePodcastUpdater>>().Object, _contextFactory);
        }

        [Fact]
        public async Task UpdateDataAsync_Should_Update_BasePodcasts()
        {
            // Arrange: Generate mock base podcasts
            var mockBasePodcasts = new List<BasePodcast>
            {
                new BasePodcast { Id = 1, Title = "Podcast 1" },
                new BasePodcast { Id = 2, Title = "Podcast 2" }
                // Add more mock base podcasts as needed
            };
        
            // Mock the method to return mockBasePodcasts
            var podcastGeneratorMock = new Mock<IPodcastGenerator>();
            podcastGeneratorMock.Setup(g => g.GenerateMockBasePodcasts(It.IsAny<int>())).Returns(mockBasePodcasts);

            // Act: Invoke the method to update base podcasts
            await _updater.UpdateDataAsync();

            // Assert: Verify the state after the update
            // You can assert against the database context to check if the base podcasts are updated as expected
            // For example:
            var updatedBasePodcasts = await _dbContext.BasePodcasts.ToListAsync();
            Assert.Equal(mockBasePodcasts.Count, updatedBasePodcasts.Count);
            // Add more assertions as needed
        }
    }
}