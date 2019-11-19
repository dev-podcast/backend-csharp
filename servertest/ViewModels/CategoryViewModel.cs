using devpodcasts.domain.entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace devpodcasts.server.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<PodcastCategoryViewModel> PodcastCategories { get; set; }
        //public ICollection<EpisodeCategory> EpisodeCategories { get; set; }

    }
}
