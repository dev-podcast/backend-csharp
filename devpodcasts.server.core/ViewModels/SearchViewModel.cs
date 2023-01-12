using System.Collections.Generic;

namespace devpodcasts.Server.Core.ViewModels
{
    public class SearchViewModel
    {
        public List<EpisodeViewModel> Episodes { get; set; }
        public List<PodcastViewModel> Podcasts { get; set; }
        public CategoryViewModel Category { get; set; }
    }
}