using devpodcasts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace devpodcasts.Domain.Interfaces;

public interface IDbContext
{
    // Add DbSet properties for other entities as needed
    DbSet<BasePodcast> BasePodcast { get; }
    DbSet<Podcast> Podcast { get; }
    DbSet<Episode> Episode { get; }
    DbSet<Tag> Tag { get; }
    DbSet<Category> Category { get; }
}