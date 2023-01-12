using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevPodcast.Domain.Entities
{
    public partial class Tag
    {
        public Tag()
        {
            Episodes = new List<Episode>();
            Podcasts = new List<Podcast>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }

        public ICollection<Episode> Episodes { get; set; }
        public ICollection<Podcast> Podcasts { get; set; }
    }
}
