using DevPodcast.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevPodcast.Data.EntityFramework
{
    public interface IApplicationDbContext
    {
        DbSet<BasePodcast> BasePodcast { get; set; }
        DbSet<Episode> Episode { get; set; }
        DbSet<Podcast> Podcast { get; set; }
        DbSet<Category> Category { get; set; }
        DbSet<Tag> Tag { get; set; }
    }
}