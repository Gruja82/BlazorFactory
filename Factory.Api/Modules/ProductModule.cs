using Factory.Api.Repositories.UoW;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Factory.Api.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Product entity
    public static class ProductModule
    {
        public static void MapProductEndpoint(this WebApplication app)
        {
            // GET handler method for returning paginated list of ProductDto objects
            app.MapGet("api/products", async ([FromServices] IUnitOfWork unitOfWork, [FromQuery] string? searchText, [FromQuery] string? category,
                [FromQuery] int pageIndex, [FromQuery] int pageSize) =>
            {
                // Invoke ProductRepository's method for returning
                // collection of ProductDto objects
                var paginatedResult = await unitOfWork.ProductRepository.GetProductsCollectionAsync(searchText ?? string.Empty, category ?? string.Empty, pageIndex, pageSize);

                return Results.Ok(paginatedResult);
            });

            // GET handler method for returning single ProductDto object
            app.MapGet("api/products/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke ProductRepository's method for returning
                // single ProductDto object
                ProductDto? productDto = await unitOfWork.ProductRepository.GetSingleProductAsync(id);

                // If productDto is not null, then return OK result
                // along with productDto.
                // Otherwise return NotFound (404) result
                return productDto != null ? Results.Ok(productDto) : Results.NotFound();
            });

            // POST handler method for creating new Product
            app.MapPost("api/products/create", async ([FromServices] IUnitOfWork unitOfWork, ProductDto productDto) =>
            {
                // Validate productDto using ProductRepository's
                // method ValidateProductAsync
                var errorCheck = await unitOfWork.ProductRepository.ValidateProductAsync(productDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke ProductRepository's method for creating new Product
                    await unitOfWork.ProductRepository.CreateNewProductAsync(productDto);

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

            // PATCH handler method for editing selected Product
            app.MapPatch("api/products/patch", async ([FromServices] IUnitOfWork unitOfWork, ProductDto productDto) =>
            {
                // Validate productDto using ProductRepository's
                // method ValidateProductAsync
                var errorCheck = await unitOfWork.ProductRepository.ValidateProductAsync(productDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke ProductRepository's method for editing selected Product
                    await unitOfWork.ProductRepository.EditProductAsync(productDto);

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

            // DELETE handler method for deleting selected Product
            app.MapDelete("api/products/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke ProductRepository's method for deleting selected Product
                    await unitOfWork.ProductRepository.DeleteProductAsync(id);

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

            // GET handler method for returning list of all ProductDto objects
            app.MapGet("api/products/all", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke ProductRepository's method for returning list of all Products
                List<ProductDto>? productList = await unitOfWork.ProductRepository.GetAllProductsAsync();

                // If productList is not null, then return OK result
                // along with productList.
                // Otherwise return new List<ProductDto>
                return productList != null ? Results.Ok(productList) : Results.NotFound();
            });
        }
    }
}
