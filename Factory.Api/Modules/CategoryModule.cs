using Factory.Api.Repositories.UoW;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Factory.Api.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Category entity
    public static class CategoryModule
    {
        public static void MapCategoryEndpoint(this WebApplication app)
        {
            // GET handler method for returning paginated list of CategoryDto objects
            app.MapGet("api/categories", async ([FromServices] IUnitOfWork unitOfWork, [FromQuery] string? searchText, [FromQuery] int pageIndex, [FromQuery] int pageSize) =>
            {
                // Invoke CategoryRepository's method for returning
                // collection of CategoryDto objects
                var paginatedResult = await unitOfWork.CategoryRepository.GetCategoriesCollectionAsync(searchText ?? string.Empty, pageIndex, pageSize);

                return Results.Ok(paginatedResult);
            });

            // GET handler method for returning single CategoryDto object
            app.MapGet("api/categories/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke CategoryRepository's method for returning
                // single CategoryDto object
                CategoryDto? categoryDto = await unitOfWork.CategoryRepository.GetSingleCategoryAsync(id);

                // If categoryDto is not null, then return OK result
                // along with categoryDto.
                // Otherwise return NotFound (404) result
                return categoryDto != null ? Results.Ok(categoryDto) : Results.NotFound();
            });

            // POST handler method for creating new Category
            app.MapPost("api/categories/create", async ([FromServices] IUnitOfWork unitOfWork, CategoryDto categoryDto) =>
            {
                // Validate categoryDto using CategoryRepository's
                // method ValidateCategoryAsync
                var errorCheck = await unitOfWork.CategoryRepository.ValidateCategoryAsync(categoryDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke CategoryRepository's method for creating new Category
                    await unitOfWork.CategoryRepository.CreateNewCategoryAsync(categoryDto);

                    // Save changes to database
                    await unitOfWork.ConfirmChangesAsync();
                    // Return status code Created (201)
                    return Results.Created();
                }
                catch (Exception)
                {
                    // If there is any exception,
                    // rollback any changes made on entities
                    unitOfWork.RollBackChanges();
                    return Results.StatusCode(500);
                }
            });

            // PATCH handler method for editing selected Category
            app.MapPatch("api/categories/patch", async ([FromServices] IUnitOfWork unitOfWork, CategoryDto categoryDto) =>
            {
                // Validate categoryDto using CategoryRepository's
                // method ValidateCategoryAsync
                var errorCheck = await unitOfWork.CategoryRepository.ValidateCategoryAsync(categoryDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke CategoryRepository's method for creating new Category
                    await unitOfWork.CategoryRepository.EditCategoryAsync(categoryDto);

                    // Save changes to database
                    await unitOfWork.ConfirmChangesAsync();
                    // Return status code No Content (204)
                    return Results.NoContent();
                }
                catch (Exception)
                {
                    // If there is any exception,
                    // rollback any changes made on entities
                    unitOfWork.RollBackChanges();
                    return Results.StatusCode(500);
                }
            });

            // DELETE handler method for deleting selected Category
            app.MapDelete("api/categories/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke CategoryRepository's method for deleting selected Category
                    await unitOfWork.CategoryRepository.DeleteCategoryAsync(id);

                    // Save changes to database
                    await unitOfWork.ConfirmChangesAsync();
                    // Return status code No Content (204)
                    return Results.NoContent();
                }
                catch (Exception)
                {
                    // If there is any exception,
                    // rollback any changes made on entities
                    unitOfWork.RollBackChanges();
                    return Results.StatusCode(500);
                }
            });

            // GET handler method for returning all Category records
            app.MapGet("api/categories/all", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke CategoryRepository's method for returning
                // collection of all CategoryDto objects
                var response = await unitOfWork.CategoryRepository.GetAllCategoriesAsync();

                return Results.Ok(response);
            });
        }
    }
}
