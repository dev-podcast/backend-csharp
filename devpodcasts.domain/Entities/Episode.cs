using System;
using System.Collections.Generic;

namespace DevPodcast.Domain.Entities
{
    public partial class Episode
    {
        public Episode()
        {
            EpisodeTags = new HashSet<EpisodeTag>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int PodcastId { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; }
        public string AudioUrl { get; set; }
        public string AudioType { get; set; }
        public string AudioDuration { get; set; }
        public DateTime? PublishedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ImageUrl { get; set; }
        public string SourceUrl { get; set; }

        public Podcast Podcast { get; set; }
        public ICollection<EpisodeTag> EpisodeTags { get; set; }
        public ICollection<EpisodeCategory> EpisodeCategories { get; set; }
    }
}