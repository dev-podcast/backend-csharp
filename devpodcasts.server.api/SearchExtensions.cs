using devpodcasts.Domain;
using devpodcasts.server.api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace devpodcasts.server.api;

public static class SearchExtensions
{
    public static void SearchEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/v1/search").WithTags("Search");

        group.MapGet("/{searchTerm}", async ([FromServices] IUnitOfWork unitOfWork, string searchTerm) =>
        {
            var result = await unitOfWork.SearchRepository.GetSearchResultAsync(unitOfWork, searchTerm);
            return Results.Ok(result.ToSearchResultDto());
        })
        .WithName("GetSearch")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Searches for podcasts and episodes.";
            operation.Description = "Returns a combined result of podcasts and episodes matching the search term.";
            return operation;
        });
    }
}
