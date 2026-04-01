using devpodcasts.Domain;
using devpodcasts.server.api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace devpodcasts.server.api;

public static class CategoryExtensions
{
    public static void CategoryEndpoints(this WebApplication app)
    {
        var categoryGroup = app.MapGroup("/v1/category").WithTags("Categories");

        categoryGroup.MapGet("/all", async ([FromServices] IUnitOfWork unitOfWork) =>
        {
            var categories = await unitOfWork.CategoryRepository.GetAllAsync();
            return Results.Ok(categories.ToCategoryDtos());
        })
        .WithName("GetAllCategories")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Retrieves all categories.";
            return operation;
        });

        categoryGroup.MapGet("/{id:guid}", async ([FromServices] IUnitOfWork unitOfWork, Guid id) =>
        {
            var category = await unitOfWork.CategoryRepository.GetAsync(c => c.Id == id);
            return category != null ? Results.Ok(category.ToCategoryDto()) : Results.NotFound();
        })
        .WithName("GetCategoryById")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Retrieves a specific category by ID.";
            return operation;
        });

        categoryGroup.MapGet("/{categoryName}", async ([FromServices] IUnitOfWork unitOfWork, string categoryName) =>
        {
            var category = await unitOfWork.CategoryRepository.GetAsync(c => c.Description == categoryName);
            return category != null ? Results.Ok(category.ToCategoryDto()) : Results.NotFound();
        })
        .WithName("GetCategoryByName")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Retrieves a specific category by name.";
            return operation;
        });
    }
}
