using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devpodcasts.server.ViewModels
{
    public class PodcastCategoryViewModel
    {
        public int CategoryId { get; set; }
        public int PodcastId { get; set; }
        public CategoryViewModel Category { get; set; }
        public PodcastViewModel Podcast { get; set; }
    }
}
