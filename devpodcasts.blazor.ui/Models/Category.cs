namespace devpodcasts.blazor.ui.Models;

public class Category
{
    public Guid Id { get; set; }    
    public string Description { get; set; }
    public ICollection<Guid> Podcasts { get; set; }
    public ICollection<Guid> Episodes { get; set; }
}