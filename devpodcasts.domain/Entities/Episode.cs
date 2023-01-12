using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace devpodcasts.Domain.Entities
{
    public partial class Episode
    {
        public Episode()
        {
            Tags = new HashSet<Tag>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string Title { get; set; }
        [MaxLength(250)]
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; }
        public string AudioUrl { get; set; }
        [MaxLength(10)]
        public string AudioType { get; set; }
        [MaxLength(10)]
        public string AudioDuration { get; set; }
        public DateTime? PublishedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ImageUrl { get; set; }
        public string SourceUrl { get; set; }
        [ForeignKey("Podcast")]
        public int PodcastId { get; set; }
        public Podcast Podcast { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}