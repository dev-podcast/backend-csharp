using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace devpodcasts.Domain.Entities
{
    public class Category
    {

        [Key]
        public Guid Id { get; set; }    
        public string Description { get; set; }
        public ICollection<Podcast> Podcasts { get; set; }
        public ICollection<Episode> Episodes { get; set; }
    }
}