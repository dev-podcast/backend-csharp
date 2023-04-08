using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace devpodcasts.common.Extensions
{
    public static class StringCleanerExtensions
    {
        private const string UppercasePodcastName = "Podcast";
        private const string LowercasePodcastName = "podcast";
        private const string UppercaseTitleName = "Title";
        private const string LowercaseTitleName = "title";

        public static string CleanHtml(this string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty).Trim();
        }

        public static string RemovePodcastFromName(this string name)
        {
            return name
                .Replace(UppercasePodcastName, "", StringComparison.InvariantCulture)
                .Replace(LowercasePodcastName, "", StringComparison.InvariantCulture)
                .Trim();
        }

        public static string RemoveTitleFromName(this string name)
        {
            return name
                .Replace(UppercaseTitleName, "", StringComparison.InvariantCulture)
                .Replace(LowercaseTitleName, "", StringComparison.InvariantCulture)
                .Trim();
        }

        public static string CleanUpTitle(this string name)
        {
            return name
                .RemovePodcastFromName()
                .RemoveTitleFromName()
                .Trim();
        }
    }
}
