using Bogus;
using devpodcasts.Domain.Entities;

namespace devpodcasts.data.mock;


public interface IPodcastGenerator
{
    List<BasePodcast> GenerateMockBasePodcasts(int count);
    List<Category> GenerateMockCategories(int count);
    List<Tag> GenerateMockTags(int count);
    List<Episode> GenerateMockEpisodes(int count);
    List<Podcast> GenerateMockPodcasts(int count);
}

public class PodcastGenerator : IPodcastGenerator
{
    public List<BasePodcast> GenerateMockBasePodcasts(int count)
    {
        var podcastFaker = new Faker<BasePodcast>()
            .RuleFor(p => p.Id, f => Guid.NewGuid())
            .RuleFor(p => p.Title, f => f.Company.CompanyName())
            .RuleFor(p => p.Description, f => f.Lorem.Sentence())
            .RuleFor(p => p.PodcastSite, f => f.Lorem.Sentence());

        return podcastFaker.Generate(count);
    }

    public List<Category> GenerateMockCategories(int count)
    {
        var categoryFaker = new Faker<Category>()
            .RuleFor(c => c.Id, f => Guid.NewGuid())
            .RuleFor(c => c.Description, f => f.Commerce.Categories(1)[0]);

        return categoryFaker.Generate(count);
    }

    public List<Tag> GenerateMockTags(int count)
    {
        var tagFaker = new Faker<Tag>()
            .RuleFor(t => t.Id, f => Guid.NewGuid())
            .RuleFor(t => t.Description, f => f.Commerce.ProductAdjective());

        return tagFaker.Generate(count);
    }

    public List<Episode> GenerateMockEpisodes(int count)
    {
        var episodeFaker = new Faker<Episode>()
            .RuleFor(e => e.Id, f => Guid.NewGuid())
            .RuleFor(e => e.Title, f => f.Commerce.ProductName())
            .RuleFor(e => e.Description, f => f.Lorem.Sentence())
            .RuleFor(e => e.PublishedDate, f => f.Date.Past())
            .RuleFor(e => e.Categories, f => GenerateMockCategories(2))
            .RuleFor(e => e.Tags, f => GenerateMockTags(3));

        return episodeFaker.Generate(count);
    }

    public List<Podcast> GenerateMockPodcasts(int count)
    {
        var podcastFaker = new Faker<Podcast>()
            .RuleFor(p => p.Id, f => Guid.NewGuid())
            .RuleFor(p => p.Title, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Lorem.Sentence())
            .RuleFor(p => p.ImageUrl, f => f.Internet.Url())
            .RuleFor(p => p.ShowUrl, f => f.Internet.Url())
            .RuleFor(p => p.FeedUrl, f => f.Internet.Url())
            .RuleFor(p => p.LatestReleaseDate, f => f.Date.Past())
            .RuleFor(p => p.EpisodeCount, f => f.Random.Number(1, 100))
            .RuleFor(p => p.Country, f => f.Address.Country())
            .RuleFor(p => p.CreatedDate, f => f.Date.Past())
            .RuleFor(p => p.Artists, f => f.Person.FullName)
            .RuleFor(p => p.ItunesId, f => f.Random.Guid().ToString())
            .RuleFor(p => p.Episodes, f => GenerateMockEpisodes(3))
            .RuleFor(p => p.Tags, f => GenerateMockTags(2))
            .RuleFor(p => p.Categories, f => GenerateMockCategories(2));

        return podcastFaker.Generate(count);
    }
}
