namespace devpodcasts.server.api.Models;

public class TagDto
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public ICollection<Guid> Podcasts { get; set; } = new List<Guid>();
    public ICollection<Guid> Episodes { get; set; } = new List<Guid>();
}
