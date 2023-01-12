using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace devpodcasts.Domain.Entities
{
    public partial class Podcast
    {
        public Podcast()
        {
            Episodes = new List<Episode>();
            Tags = new List<Tag>();
            Categories = new List<Category>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public string ShowUrl { get; set; }
        public string FeedUrl { get; set; }
        [Column(TypeName = "DateTime")]
        public DateTime LatestReleaseDate { get; set; }
        public int EpisodeCount { get; set; }
        [MaxLength(50)]
        public string Country { get; set; }
        [Column(TypeName ="DateTime")]
        public DateTime CreatedDate { get; set; }
        public string Artists { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string ItunesId { get; set; }
        public ICollection<Episode> Episodes { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}