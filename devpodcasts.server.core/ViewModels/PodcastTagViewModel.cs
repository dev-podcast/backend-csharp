namespace DevPodcast.Server.ViewModels
{
    public class PodcastTagViewModel
    {
        public int TagId { get; set; }
        public int PodcastId { get; set; }

        public PodcastViewModel Podcast { get; set; }
        public TagViewModel Tag { get; set; }
    }
}