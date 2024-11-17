using Factory.Api.Repositories.UoW;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Factory.Api.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Production entity
    public static class ProductionModule
    {
        public static void MapProductionEndpoint(this WebApplication app)
        {
            // GET handler method for returning paginted list of ProductionDto objects
            app.MapGet("api/productions", async ([FromServices] IUnitOfWork unitOfWork, [AsParameters] QueryStringParamsProduction queryStrings) =>
            {
                // Invoke ProductionRepository's method for returning
                // collection of ProductionDto objects
                var paginatedResult = await unitOfWork.ProductionRepository.GetProductionsCollectionAsync(queryStrings.SearchText ?? string.Empty, queryStrings.StringDate ?? string.Empty,
                    queryStrings.ProductName ?? string.Empty, queryStrings.PageIndex, queryStrings.PageSize);

                return Results.Ok(paginatedResult);
            });

            // GET handler method for returning single ProductionDto object
            app.MapGet("api/productions/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke ProductionRepository's method for returning
                // single ProductionDto object
                ProductionDto productionDto = await unitOfWork.ProductionRepository.GetSingleProductionAsync(id);

                // If productionDto is not null, then return OK result
                // along with productionDto.
                // Otherwise return NotFound (404) result
                return productionDto != null ? Results.Ok(productionDto) : Results.NotFound();
            });

            // POST handler method for creating new Production
            app.MapPost("api/productions/create", async ([FromServices] IUnitOfWork unitOfWork, ProductionDto productionDto) =>
            {
                // Validate productionDto using ProductionRepository's
                // method ValidateProductionAsync
                var errorCheck = await unitOfWork.ProductionRepository.ValidateProductionAsync(productionDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke ProductionRepository's method for creating new Production
                    await unitOfWork.ProductionRepository.CreateNewProductionAsync(productionDto);

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

            // PATCH handler method for editing selected Production
            app.MapPatch("api/productions/patch", async ([FromServices] IUnitOfWork unitOfWork, ProductionDto productionDto) =>
            {
                // Validate productionDto using ProductionRepository's
                // method ValidateProductionAsync
                var errorCheck = await unitOfWork.ProductionRepository.ValidateProductionAsync(productionDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke ProductionRepository's method for editing selected Production
                    await unitOfWork.ProductionRepository.EditProductionAsync(productionDto);

                    // Save changes to database
                    await unitOfWork.ConfirmChangesAsync();
                    // Return status code No Content (204)
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

            // DELETE handler method for deleting selected Production
            app.MapDelete("api/productions/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke ProductionRepository's method for deleting selected Production
                    await unitOfWork.ProductionRepository.DeleteProductionAsync(id);

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

            // GET handler method for returning list of all Production dates
            app.MapGet("api/productions/dates", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke ProductionRepository's method for returning
                // list of Production dates
                var productionDates = await unitOfWork.ProductionRepository.GetAllProductionDatesAsync();

                return Results.Ok(productionDates);
            });
        }
    }
}
