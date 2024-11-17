using Factory.Api.Repositories.UoW;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Factory.Api.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Material entity
    public static class MaterialModule
    {
        public static void MapMaterialEndpoint(this WebApplication app)
        {
            // GET handler method for returning paginated list of MaterialDto objects
            app.MapGet("api/materials", async ([FromServices] IUnitOfWork unitOfWork, [FromQuery] string? searchText, [FromQuery] string? category,
                [FromQuery] int pageindex, [FromQuery] int pageSize) =>
            {
                // Invoke MaterialRepository's method for returning
                // collection of MaterialDto objects
                var paginatedResult = await unitOfWork.MaterialRepository.GetMaterialsCollectionAsync(searchText ?? string.Empty, category ?? string.Empty, pageindex, pageSize);

                return Results.Ok(paginatedResult);
            });

            // GET handler method for returning single MaterialDto object
            app.MapGet("api/materials/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke MaterialRepository's method for returning
                // single MaterialDto object
                MaterialDto? materialDto = await unitOfWork.MaterialRepository.GetSingleMaterialAsync(id);

                // If materialDto is not null, then return OK result
                // along with materialDto.
                // Otherwise return NotFound (404) result
                return materialDto != null ? Results.Ok(materialDto) : Results.NotFound();
            });

            // POST handler method for creating new Material
            app.MapPost("api/materials/create", async ([FromServices] IUnitOfWork unitOfWork, MaterialDto materialDto) =>
            {
                // Validate materialDto using MaterialRepository's
                // method ValidateMaterialAsync
                var errorCheck = await unitOfWork.MaterialRepository.ValidateMaterialAsync(materialDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke MaterialRepository's method for creating new Material
                    await unitOfWork.MaterialRepository.CreateNewMaterialAsync(materialDto);

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

            // PATCH handler method for editing selected Material
            app.MapPatch("api/materials/patch", async ([FromServices] IUnitOfWork unitOfWork, MaterialDto materialDto) =>
            {
                // Validate materialDto using MaterialRepository's
                // method ValidateMaterialAsync
                var errorCheck = await unitOfWork.MaterialRepository.ValidateMaterialAsync(materialDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke MaterialRepository's method for editing selected Material
                    await unitOfWork.MaterialRepository.EditMaterialAsync(materialDto);

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

            // DELETE handler method for deleting selected Material
            app.MapDelete("api/materials/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke MaterialRepository's method for deleting selected Material
                    await unitOfWork.MaterialRepository.DeleteMaterialAsync(id);
                }
                catch (Exception)
                {
                    // If there is any exception,
                    // rollback any changes made on entities
                    unitOfWork.RollBackChanges();
                    return Results.StatusCode(500);
                }

                // Save changes to database
                await unitOfWork.ConfirmChangesAsync();
                // Return status code No Content (204)
                return Results.NoContent();
            });

            // GET handler method for returning all Material records
            app.MapGet("api/materials/all", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke MaterialRepository's method for returning
                // collection of all MaterialDto objects
                var response = await unitOfWork.MaterialRepository.GetAllMaterialsAsync();

                return Results.Ok(response);
            });
        }
    }
}
