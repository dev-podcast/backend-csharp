using DevPodcast.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevPodcast.Data.EntityFramework
{
    public interface IApplicationDbContext
    {
        DbSet<BasePodcast> BasePodcast { get; set; }
        DbSet<Episode> Episode { get; set; }
        DbSet<EpisodeTag> EpisodeTag { get; set; }
        DbSet<Podcast> Podcast { get; set; }
        DbSet<PodcastTag> PodcastTag { get; set; }
        DbSet<PodcastCategory> PodcastCategory { get; set; }
        DbSet<EpisodeCategory> EpisodeCategory { get; set; }
        DbSet<Category> Category { get; set; }
        DbSet<Tag> Tag { get; set; }
    }
}