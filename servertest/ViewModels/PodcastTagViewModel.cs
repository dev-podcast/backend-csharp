using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devpodcasts.server.ViewModels
{
    public class PodcastTagViewModel
    {
        public int TagId { get; set; }
        public int PodcastId { get; set; }

        public PodcastViewModel Podcast { get; set; }
        public TagViewModel Tag { get; set; }
    }
}
