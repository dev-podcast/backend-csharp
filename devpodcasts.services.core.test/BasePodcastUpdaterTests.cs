using System.Linq;
using DevPodcast.Services.Core.JsonObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using SharpTestsEx;

namespace DevPodcast.Services.Core.Test
{
    public class BasePodcastUpdaterTests
    {

        private string podListJSON =
            "{\r\n  \"General Interest\": [\r\n    {\r\n      \"Title\": \"Simple Programmer Podcast\",\r\n      \"PodcastSite\": \"https://simpleprogrammer.com\",\r\n      \"Description\": \"John Sonmez wants to help you become more successful, make more money, deal with difficult coworkers and be so productive everyone thinks you must be abusing a prescription for Ritalin. Listen in every Monday, Wednesday and Friday, as Simple Programmer founder, John Sonmez, answers questions, interviews guests and shares everything he knows to help you become a top performing software developer. The Simple Programmer Podcast is a short podcast that is a mix of career advice, philosophy and soft skills from successful author and software developer, John Sonmez. John is the founder of http://simpleprogrammer.com , one of the most popular software development blogs, and the author of the best-selling book, \\\"Soft Skills: The Software Developer\'s Life Manual.\\\" (http://simpleprogrammer.com/softskills ) Geared towards a programmer or software developer audience, but contains practical advice on: Career development Entrepreneurship Fitness Finance Productivity Personal development And more... That anyone can benefit from. Each episode is between 5 and 10 minutes long with at least 3 new episodes each week.\",\r\n      \"ItunesSubscriberUrl\": \"https://itunes.apple.com/us/podcast/simple-programmer-podcast/id998357224\",\r\n      \"Episodes John Was On\": \"All of them!\"\r\n    },\r\n    {\r\n      \"Title\": \"Get Up and CODE\",\r\n      \"PodcastSite\": \"http://getupandcode.com/\",\r\n      \"Description\": \"This podcast is all about fitness and nutrition with an IT slant. If you are interested in technology and fitness, this podcast might be exactly what you are looking for. The podcast started with John Sonmez and Iris Classon in 2013, two experienced developers as well as fitness geeks. In 2015 Robert Navarro started hosting the podcast after John Sonmez handed him the keys to the castle and allowed him to continue the tradition of helping others reach both their fitness and career goals. In this podcast they share all they know about getting in shape, losing weight, gaining muscle and eating right in their weekly discussion about all things fitness and nutrition. Want to know how to really get those 6 pack abs? Are carbs really bad for you? John and Iris answer all these questions and more in this short weekly podcast with about fitness and nutrition from engineering minds. Here is a short sampling for some of the topics discussed in this podcast:: fitness, nutrition, software development, programming, muscle, weight training, lifting, running, 5ks, marathons, jogging, workout, health, healthy living, aerobics, weight loss, exercise, diet, motivation.\",\r\n      \"ItunesSubscriberUrl\": \"https://itunes.apple.com/us/podcast/get-up-and-code!/id646958161?mt=2\",\r\n      \"Episodes John Was On\": \"Just about all of them!\"\r\n    }\r\n    ]\r\n}";

        [Fact]
        public void When_podlist_json_is_read_Then_deserialize_to_RootJsonObject()
        {
            var json = JToken.Parse(podListJSON);
            
            var basePodcasts = JsonConvert.DeserializeObject<RootJsonObject>(json.ToString());
            basePodcasts.GeneralInterest.Count().Should().Be.EqualTo(2);
        }
    }
}