using System;
using System.Collections.Generic;
using System.Text;

namespace DevPodcast.Domain.Entities
{
    public class SearchResult
    {
        public ICollection<Episode> Episodes { get; set; }
        public ICollection<Podcast> Podcasts { get; set; }
        public Category Category { get; set; } = new Category();
        
    }
}
