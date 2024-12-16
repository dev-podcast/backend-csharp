using devpodcasts.data.mock.Extensions;
using devpodcasts.Domain.Entities;
using devpodcasts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace devpodcasts.data.mock;




// Mock implementation of the database context using the PodcastGenerator
public class MockDbContext : DbContext, IDbContext
{
    private readonly IPodcastGenerator _podcastGenerator;

    public MockDbContext(IPodcastGenerator podcastGenerator)
    {
        _podcastGenerator = podcastGenerator;
    }

    public DbSet<BasePodcast> BasePodcast => _podcastGenerator.GenerateMockBasePodcasts(30).AsQueryable().BuildMockDbSet();

    public DbSet<Podcast> Podcast => _podcastGenerator.GenerateMockPodcasts(30).AsQueryable().BuildMockDbSet();

    public DbSet<Episode> Episode => _podcastGenerator.GenerateMockEpisodes(30).AsQueryable().BuildMockDbSet();

    public DbSet<Tag> Tag => _podcastGenerator.GenerateMockTags(20).AsQueryable().BuildMockDbSet();

    public DbSet<Category> Category => _podcastGenerator.GenerateMockCategories(20).AsQueryable().BuildMockDbSet();
// Implement other DbSet properties using the PodcastGenerator as needed
}