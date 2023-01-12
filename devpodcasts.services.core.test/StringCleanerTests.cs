using DevPodcast.Services.Core.Utils;
using SharpTestsEx;
using Xunit;

namespace DevPodcast.Services.Core.Test
{
    public class StringCleanerTests
    {
        [Theory]
        [InlineData("<body>Get up and Code Podcast podcast</body>")]
        public void When_string_includes_html_Then_remove_it(string value)
        {
            var cleanedString = value.CleanHtml();
            cleanedString.Should().Be.EqualTo("Get up and Code Podcast podcast");
        }


        [Theory]
        [InlineData("Get up and Code Podcast podcast")]
        public void When_string_contains_the_word_podcast_Then_remove_it(string value)
        {
            var cleanedString = value.RemovePodcastFromName();
            cleanedString.Should().Be.EqualTo("Get up and Code");
        }

        [Theory]
        [InlineData("Get up and Code Title title")]
        public void When_string_contains_the_word_title_Then_remove_it(string value)
        {
            var cleanedString = value.RemoveTitleFromName();
            cleanedString.Should().Be.EqualTo("Get up and Code");
        }
    }
}