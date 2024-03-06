using devpodcasts.data.mock.Extensions;
using devpodcasts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace devpodcasts.data.mock;


public interface IDbContext
{
    DbSet<BasePodcast> BasePodcasts { get; }
    DbSet<Podcast> Podcasts
    // Add DbSet properties for other entities as needed
}

// Mock implementation of the database context using the PodcastGenerator
public class MockDbContext : DbContext, IDbContext
{
    private readonly IPodcastGenerator _podcastGenerator;

    public MockDbContext(IPodcastGenerator podcastGenerator)
    {
        _podcastGenerator = podcastGenerator;
    }

    public DbSet<BasePodcast> BasePodcasts => _podcastGenerator.GenerateMockBasePodcasts(30).AsQueryable().BuildMockDbSet();

    public DbSet<Podcast> Podcasts => _podcastGenerator.GenerateMockPodcasts(30).AsQueryable().BuildMockDbSet();

    public DbSet<Episode> Episodes => _podcastGenerator.GenerateMockEpisodes(30).AsQueryable().BuildMockDbSet();

    public DbSet<Tag> Tags => _podcastGenerator.GenerateMockTags(20).AsQueryable().BuildMockDbSet();

    public DbSet<Category> Categories => _podcastGenerator.GenerateMockCategories(20).AsQueryable().BuildMockDbSet();
// Implement other DbSet properties using the PodcastGenerator as needed
}