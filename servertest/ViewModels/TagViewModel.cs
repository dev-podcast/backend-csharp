using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devpodcasts.server.ViewModels
{
    public class TagViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public ICollection<EpisodeTagViewModel> EpisodeTags { get; set; }
        public ICollection<PodcastTagViewModel> PodcastTags { get; set; }
    }
}
