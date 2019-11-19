using System.Collections.Generic;

namespace DevPodcast.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public ICollection<PodcastCategory> PodcastCategories { get; set; }
        public ICollection<EpisodeCategory> EpisodeCategories { get; set; }
    }
}