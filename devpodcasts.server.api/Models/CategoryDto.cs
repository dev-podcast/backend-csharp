namespace devpodcasts.server.api.Models;

public class CategoryDto
{
    public Guid Id { get; set; }    
    public string Description { get; set; }
    public ICollection<Guid> Podcasts { get; set; }
    public ICollection<Guid> Episodes { get; set; }
}