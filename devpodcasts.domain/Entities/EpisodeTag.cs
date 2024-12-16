using System;

namespace devpodcasts.Domain.Entities
{
    public partial class EpisodeTag
    {
        public Guid TagId { get; set; }
        public Guid EpisodeId { get; set; }

        public Episode Episode { get; set; }
        public Tag Tag { get; set; }
    }
}