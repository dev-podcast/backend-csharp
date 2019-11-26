using System;
using System.Collections.Generic;

namespace DevPodcast.Server.ViewModels
{
    public class PodcastViewModel
    {
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

        public ICollection<EpisodeViewModel> Episodes { get; set; }
        public ICollection<PodcastTagViewModel> Tags { get; set; }
        public ICollection<PodcastCategoryViewModel> Categories { get; set; }
    }
}