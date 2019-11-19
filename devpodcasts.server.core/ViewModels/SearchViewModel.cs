using System.Collections.Generic;

namespace DevPodcast.Server.ViewModels
{
    public class SearchViewModel
    {
        public List<EpisodeViewModel> Episodes { get; set; }
        public List<PodcastViewModel> Podcasts { get; set; }
        public CategoryViewModel Category { get; set; }
    }
}