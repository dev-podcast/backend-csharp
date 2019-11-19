using System;
using System.Text.RegularExpressions;

namespace DevPodcast.Services.Core.Utils
{
    public static class StringCleaner
    {
        private const string UppercasePodcastName = "Podcast";
        private const string LowercasePodcastName = "podcast";
        private const string UppercaseTitleName = "Title";
        private const string LowercaseTitleName = "title";

        public static  string CleanHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static string RemovePodcastFromName(this string name)
        {
            return name
                .Replace(UppercasePodcastName, "", StringComparison.InvariantCulture)
                .Replace(LowercasePodcastName, "", StringComparison.InvariantCulture);
        }

        public static string RemoveTitleFromName(this string name)
        {
            return name
                .Replace(UppercaseTitleName, "", StringComparison.InvariantCulture)
                .Replace(LowercaseTitleName, "", StringComparison.InvariantCulture);
        }

        public static string CleanUpTitle(this string name)
        {
            return name
                .RemovePodcastFromName()
                .RemoveTitleFromName();
        }
    }
}