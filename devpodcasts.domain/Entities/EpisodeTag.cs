namespace devpodcasts.Domain.Entities
{
    public partial class EpisodeTag
    {
        public int TagId { get; set; }
        public int EpisodeId { get; set; }

        public Episode Episode { get; set; }
        public Tag Tag { get; set; }
    }
}