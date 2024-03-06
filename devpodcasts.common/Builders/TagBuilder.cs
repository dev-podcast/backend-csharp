using devpodcasts.Domain.Entities;

namespace devpodcasts.common.Builders;

public class TagBuilder
{
    private readonly Tag _tag;

    private TagBuilder()
    {
        _tag = new Tag();
    }

    public static TagBuilder Create()
    {
        return new TagBuilder();
    }

    public TagBuilder WithId(int id)
    {
        _tag.Id = id;
        return this;
    }

    public TagBuilder WithDescription(string description)
    {
        _tag.Description = description;
        return this;
    }

    public TagBuilder WithEpisodes(ICollection<Episode> episodes)
    {
        _tag.Episodes = episodes;
        return this;
    }

    public TagBuilder WithPodcasts(ICollection<Podcast> podcasts)
    {
        _tag.Podcasts = podcasts;
        return this;
    }

    public Tag Build()
    {
        // You might want to perform additional validation or configuration here
        return _tag;
    }
}