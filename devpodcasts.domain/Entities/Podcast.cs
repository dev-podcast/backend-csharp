using System;
using System.Collections.Generic;

namespace DevPodcast.Domain.Entities
{
    public partial class Podcast
    {
        public Podcast()
        {
            Episodes = new List<Episode>();
            PodcastTags = new List<PodcastTag>();
            PodcastCategories = new List<PodcastCategory>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ShowUrl { get; set; }
        public string FeedUrl { get; set; }
        public DateTime LatestReleaseDate { get; set; }
        public int EpisodeCount { get; set; }
        public string Country { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Artists { get; set; } = string.Empty;
        public string ItunesId { get; set; }

        public ICollection<Episode> Episodes { get; set; }
        public ICollection<PodcastTag> PodcastTags { get; set; }
        public ICollection<PodcastCategory> PodcastCategories { get; set; }
    }
}