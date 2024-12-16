using System.Collections.Generic;

namespace devpodcasts.Domain.Entities.Dtos;

public class SearchResult
{
    public ICollection<Episode> Episodes { get; set; }
    public ICollection<Podcast> Podcasts { get; set; }
    public Category Category { get; set; } = new();
}