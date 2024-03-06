using devpodcasts.Domain.Entities;

namespace devpodcasts.common.Builders;

public class CategoryBuilder
{
    private readonly Category _category;

    private CategoryBuilder()
    {
        _category = new Category();
    }

    public static CategoryBuilder Create()
    {
        return new CategoryBuilder();
    }

    public CategoryBuilder WithId(int id)
    {
        _category.Id = id;
        return this;
    }

    public CategoryBuilder WithDescription(string description)
    {
        _category.Description = description;
        return this;
    }

    public CategoryBuilder WithPodcasts(ICollection<Podcast> podcasts)
    {
        _category.Podcasts = podcasts;
        return this;
    }

    public CategoryBuilder WithEpisodes(ICollection<Episode> episodes)
    {
        _category.Episodes = episodes;
        return this;
    }

    public Category Build()
    {
        // You might want to perform additional validation or configuration here
        return _category;
    }
}