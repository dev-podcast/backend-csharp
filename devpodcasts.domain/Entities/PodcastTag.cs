namespace DevPodcast.Domain.Entities
{
    public partial class PodcastTag
    {
        public int TagId { get; set; }
        public int PodcastId { get; set; }
        public Podcast Podcast { get; set; }
        public Tag Tag { get; set; }
    }
}