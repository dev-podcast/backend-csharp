using System;
using System.Collections.Generic;

namespace DevPodcast.Domain.Entities
{
    public partial class Tag
    {
        public Tag()
        {
            EpisodeTags = new List<EpisodeTag>();
            PodcastTags = new List<PodcastTag>();
        }

        public int Id { get; set; }
        public string Description { get; set; }

        public ICollection<EpisodeTag> EpisodeTags { get; set; }
        public ICollection<PodcastTag> PodcastTags { get; set; }
    }
}
