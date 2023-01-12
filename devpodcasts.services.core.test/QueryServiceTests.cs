using DevPodcast.Services.Core.Utils;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace DevPodcast.Services.Core.Test
{
    public class QueryServiceTests
    {
        [Fact]
        public async void When_Querying_For_Itunes_Data_Then_Get_Json_Data_Back()
        {
            //var itunesId = "id646958161";
            //var result = await ItunesQueryService.QueryItunesId(itunesId);
            //if (result != null)
            //{
            //    dynamic responsePod = result[0];
            //    if (responsePod != null)
            //    {
            //        string trackName = responsePod.trackName;
            //        trackName = trackName.CleanUpTitle();
                    
            //        Assert.Equal("Get Up and CODE",trackName);
            //    }
            //}
        }

        [Fact]
        public async void When_Querying_the_feed_url_Then_get_episode_data_back()
        {
            //IEnumerable<XElement> episodes = await ItunesQueryService.QueryFeedUrl("http://getupandcode1.libsyn.com/rss");
            //Assert.NotEmpty(episodes);
        }
    }
}
