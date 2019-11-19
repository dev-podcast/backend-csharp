using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devpodcasts.server.ViewModels
{
    public class SearchViewModel
    {
        public List<EpisodeViewModel> Episodes { get; set; }
        public List<PodcastViewModel> Podcasts { get; set; }
        public CategoryViewModel Category { get; set; }
    }
}
