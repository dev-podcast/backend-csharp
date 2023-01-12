using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace DevPodcast.Domain.Entities
{
    public class Category
    {

        [Key]
        public int Id { get; set; }    
        public string Description { get; set; }
        public ICollection<Podcast> Podcasts { get; set; }
        public ICollection<Episode> Episodes { get; set; }
    }
}