using devpodcasts.Services.Core.Updaters;
using devpodcasts.common.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace devpodcasts.Services.Core.Test
{
    public class EpisodeUpdaterTests //: IClassFixture<DbFixture>
    {

        //private readonly IConfiguration configuration;


        //public EpisodeUpdaterTests(DbFixture dbFixture)
        //{
        //    configuration = dbFixture.Configuration;
        //}

        //private DbContextFactory getMockDbContextFactory()
        //{
        //    var mockObject = new Mock<DbContextFactory>();
        //    return mockObject.Object;
        //}

        //private ILogger<ItunesEpisodeUpdater> getMockLogger()
        //{
        //    var mockLogger = new Mock<ILogger<ItunesEpisodeUpdater>>();
        //    return mockLogger.Object;
        //}

        //[Fact]
        //public async void When_new_episode_exists_Then_update_podcast()
        //{
        //    // await itunesEpisodeUpdater.UpdateDataAsync();
        //    //var itunesEpisodeUpdater = new ItunesEpisodeUpdater(getMockLogger(), configuration, getMockDbContextFactory());
        //    //await itunesEpisodeUpdater.UpdateDataAsync();
        //}
    }
}