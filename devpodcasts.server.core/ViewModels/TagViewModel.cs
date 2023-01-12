using System.Collections.Generic;

namespace devpodcasts.Server.Core.ViewModels
{
    public class TagViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public ICollection<EpisodeTagViewModel> EpisodeTags { get; set; }
        public ICollection<PodcastTagViewModel> PodcastTags { get; set; }
    }
}