namespace DevPodcast.Domain.Entities
{
    public class EpisodeCategory
    {
        public int CategoryId { get; set; }
        public int EpisodeId { get; set; }
        public Category Category { get; set; }
        public Episode Episode { get; set; }
    }
}