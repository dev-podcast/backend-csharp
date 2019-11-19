using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devpodcasts.server.ViewModels
{
    public class EpisodeTagViewModel
    {
        public EpisodeViewModel Episode { get; set; }
        public TagViewModel Tag { get; set; }
    }
}
