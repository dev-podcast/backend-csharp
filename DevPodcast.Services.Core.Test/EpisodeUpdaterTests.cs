using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DevPodcast.Data.EntityFramework;
using DevPodcast.Services.Core.Interfaces;
using DevPodcast.Services.Core.Utils;
using DevPodcast.Services.Core.Updaters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevPodcast.Services.Core.Test
{
    public class EpisodeUpdaterTests: IClassFixture<DbFixture>
    {

        private readonly IConfiguration configuration;


        public EpisodeUpdaterTests(DbFixture dbFixture)
        {
            configuration = dbFixture.Configuration;
        }

        private DbContextFactory getMockDbContextFactory()
        {
            var mockObject = new Mock<DbContextFactory>();
            return mockObject.Object;
        }

        private ILogger<ItunesEpisodeUpdater> getMockLogger()
        {
            var mockLogger = new Mock<ILogger<ItunesEpisodeUpdater>>();
            return mockLogger.Object;
        }

        [Fact]
        public async void When_new_episode_exists_Then_update_podcast()
        {
            // await itunesEpisodeUpdater.UpdateDataAsync();
            var itunesEpisodeUpdater = new ItunesEpisodeUpdater(getMockLogger(), configuration, getMockDbContextFactory());
            await itunesEpisodeUpdater.UpdateDataAsync();
            

        }
    }
}