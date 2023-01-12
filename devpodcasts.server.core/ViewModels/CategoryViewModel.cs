using System.Collections.Generic;

namespace devpodcasts.Server.Core.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<PodcastCategoryViewModel> PodcastCategories { get; set; } 
        //public ICollection<EpisodeCategory> EpisodeCategories { get; set; }
    }
}