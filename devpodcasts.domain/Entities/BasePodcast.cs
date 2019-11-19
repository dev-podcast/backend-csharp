namespace DevPodcast.Domain.Entities
{
    public partial class BasePodcast
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PodcastSite { get; set; }
        public string ItunesSubscriberUrl { get; set; }
        public string ItunesId { get; set; }
    }
}