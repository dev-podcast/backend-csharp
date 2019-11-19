namespace DevPodcast.Server.ViewModels
{
    public class PodcastCategoryViewModel
    {
        public int CategoryId { get; set; }
        public int PodcastId { get; set; }
        public CategoryViewModel Category { get; set; }
        public PodcastViewModel Podcast { get; set; }
    }
}