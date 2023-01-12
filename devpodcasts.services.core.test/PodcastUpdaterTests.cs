using devpodcasts.Services.Core.JsonObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpTestsEx;
using Xunit;

namespace devpodcasts.Services.Core.Test
{
    public class PodcastUpdaterTests
    {
        private string data =
            "[{\r\n    \"wrapperType\": \"track\",\r\n    \"kind\": \"podcast\",\r\n    \"collectionId\": 373016482,\r\n    \"trackId\": 373016482,\r\n    \"artistName\": \"Unknown\",\r\n    \"collectionName\": \"Lately in PHP podcast\",\r\n    \"trackName\": \"Lately in PHP podcast\",\r\n    \"collectionCensoredName\": \"Lately in PHP podcast\",\r\n    \"trackCensoredName\": \"Lately in PHP podcast\",\r\n    \"collectionViewUrl\": \"https://podcasts.apple.com/us/podcast/lately-in-php-podcast/id373016482?uo=4\",\r\n    \"feedUrl\": \"https://www.phpclasses.org/blog/category/podcast/post/latest.rss\",\r\n    \"trackViewUrl\": \"https://podcasts.apple.com/us/podcast/lately-in-php-podcast/id373016482?uo=4\",\r\n    \"artworkUrl30\": \"https://is5-ssl.mzstatic.com/image/thumb/Podcasts2/v4/49/43/a6/4943a69c-aa37-b392-bc6c-e33f05df692b/mza_2673001644819039864.jpg/30x30bb.jpg\",\r\n    \"artworkUrl60\": \"https://is5-ssl.mzstatic.com/image/thumb/Podcasts2/v4/49/43/a6/4943a69c-aa37-b392-bc6c-e33f05df692b/mza_2673001644819039864.jpg/60x60bb.jpg\",\r\n    \"artworkUrl100\": \"https://is5-ssl.mzstatic.com/image/thumb/Podcasts2/v4/49/43/a6/4943a69c-aa37-b392-bc6c-e33f05df692b/mza_2673001644819039864.jpg/100x100bb.jpg\",\r\n    \"collectionPrice\": 0.0,\r\n    \"trackPrice\": 0.0,\r\n    \"trackRentalPrice\": 0,\r\n    \"collectionHdPrice\": 0,\r\n    \"trackHdPrice\": 0,\r\n    \"trackHdRentalPrice\": 0,\r\n    \"releaseDate\": \"2017-10-18T16:53:00Z\",\r\n    \"collectionExplicitness\": \"cleaned\",\r\n    \"trackExplicitness\": \"cleaned\",\r\n    \"trackCount\": 10,\r\n    \"country\": \"USA\",\r\n    \"currency\": \"USD\",\r\n    \"primaryGenreName\": \"Technology\",\r\n    \"contentAdvisoryRating\": \"Clean\",\r\n    \"artworkUrl600\": \"https://is5-ssl.mzstatic.com/image/thumb/Podcasts2/v4/49/43/a6/4943a69c-aa37-b392-bc6c-e33f05df692b/mza_2673001644819039864.jpg/600x600bb.jpg\",\r\n\"genreIds\": [\r\n\"1318\",\r\n\"26\",\r\n\"1304\",\r\n\"1499\"\r\n],\r\n\"genres\": [\r\n\"Technology\",\r\n\"Podcasts\",\r\n\"Education\",\r\n\"How To\"\r\n]\r\n}]";


        private PodcastResult GetPodcastResult()
        {
            var jArray = JArray.Parse(data);
            return jArray.HasValues ? jArray[0].ToObject<PodcastResult>() : new PodcastResult();
        }

        [Fact]
        public void When_json_data_is_parsed_Then_return_PodcastResult_object()
        {
            var jArray = JArray.Parse(data);

            if (jArray.HasValues)
            {
                var phpPodcast = jArray[0].ToObject<PodcastResult>();
                phpPodcast.TrackName.ToLowerInvariant()
                    .Should()
                    .Be
                    .EqualTo("Lately in PHP podcast".ToLowerInvariant());
            }
           
        }
    }

    
}