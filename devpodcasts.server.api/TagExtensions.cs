using devpodcasts.Domain;
using devpodcasts.server.api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace devpodcasts.server.api;

public static class TagExtensions
{
    public static void TagEndpoints(this WebApplication app)
    {
        var tagsGroup = app.MapGroup("/v1/tags").WithTags("Tags");

        tagsGroup.MapGet("/all", async ([FromServices] IUnitOfWork unitOfWork) =>
        {
            var tags = await unitOfWork.TagRepository.GetAllAsync();
            return Results.Ok(tags.ToTagDtos());
        })
        .WithName("GetAllTags")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Retrieves all tags.";
            return operation;
        });

        tagsGroup.MapGet("/{id:guid}", async ([FromServices] IUnitOfWork unitOfWork, Guid id) =>
        {
            var tag = await unitOfWork.TagRepository.GetAsync(t => t.Id == id);
            return tag != null ? Results.Ok(tag.ToTagDto()) : Results.NotFound();
        })
        .WithName("GetTagById")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Retrieves a specific tag by ID.";
            return operation;
        });

        tagsGroup.MapGet("/{tagName}", async ([FromServices] IUnitOfWork unitOfWork, string tagName) =>
        {
            var tag = await unitOfWork.TagRepository.GetAsync(t => t.Description == tagName);
            return tag != null ? Results.Ok(tag.ToTagDto()) : Results.NotFound();
        })
        .WithName("GetTagByName")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Retrieves a specific tag by name.";
            return operation;
        });
    }
}
