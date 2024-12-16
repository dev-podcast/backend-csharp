using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using devpodcasts.common.Interfaces;
using devpodcasts.common.JsonObjects;
using Microsoft.Extensions.Configuration;
using devpodcasts.common.Updaters;
using devpodcasts.Data.EntityFramework;
using devpodcasts.data.mock;
using devpodcasts.Domain;
using devpodcasts.Domain.Entities;
using devpodcasts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace devpodcasts.Services.Core.Test
{
    public class BasePodcastUpdaterTests : IClassFixture<DbFixture>
    {
        private readonly Mock<IDbContextFactory<ApplicationDbContext>> _contextFactory;

        public BasePodcastUpdaterTests(DbFixture fixture)
        {
          
        }

        [Fact]
        public async Task UpdateDataAsync_Should_Update_BasePodcasts()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<BasePodcastUpdater>>();
            var dbContextMock = new Mock<ApplicationDbContext>(); // or use a mocking library for DbContext

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;


            var podcastGenerator = new PodcastGenerator();

            // Create a new instance of ApplicationDbContext using the in-memory database provider
            using (var dbContext = new ApplicationDbContext(options))
            {
                dbContext.BasePodcast.AddRange(podcastGenerator.GenerateMockBasePodcasts(It.IsAny<int>()));
                await dbContext.SaveChangesAsync();
                
                var contextFactoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
                contextFactoryMock.Setup(f => f.CreateDbContext()).Returns(dbContext);

                var unitOfWorkMock = new Mock<IUnitOfWork>();

                var updater = new BasePodcastUpdater(loggerMock.Object, contextFactoryMock.Object, unitOfWorkMock.Object);

                // Act
                await updater.UpdateDataAsync();
            };

            // Seed the in-memory database with test data
           

       

            // Assert
            // Add assertions based on expected behavior
        }
    }
}