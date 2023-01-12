using System;
using System.Collections.Generic;

namespace devpodcasts.Server.Core.ViewModels
{
    public class EpisodeViewModel
    {
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

        public PodcastViewModel Podcast { get; set; }
        public ICollection<EpisodeTagViewModel> Tags { get; set; }
        //public IEnumerable<EpisodeCategory> EpisodeCategories { get; set; }
    }
}