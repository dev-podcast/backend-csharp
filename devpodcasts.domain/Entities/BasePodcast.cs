﻿using System;
using System.ComponentModel.DataAnnotations;

namespace devpodcasts.Domain.Entities
{
    public partial class BasePodcast
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string PodcastSite { get; set; }
        public string ItunesSubscriberUrl { get; set; }
        [MaxLength(50)]
        public string ItunesId { get; set; }
    }
}