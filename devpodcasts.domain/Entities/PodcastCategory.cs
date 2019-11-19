namespace DevPodcast.Domain.Entities
{
    public class PodcastCategory
    {
        public int CategoryId { get; set; }
        public int PodcastId { get; set; }
        public Category Category { get; set; }
        public Podcast Podcast { get; set; }
    }
}