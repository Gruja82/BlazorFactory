using Factory.Api.Repositories.UoW;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Factory.Api.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Supplier entity
    public static class SupplierModule
    {
        public static void MapSupplierEndpoint(this WebApplication app)
        {
            // GET handler method for returning paginated list of SupplierDto objects
            app.MapGet("api/suppliers", async ([FromServices] IUnitOfWork unitOfWork, [FromQuery] string? searchText, [FromQuery] int pageIndex, [FromQuery] int pageSize) =>
            {
                // Invoke SupplierRepository's method for returning
                // collection of SupplierDto objects
                var paginatedResult = await unitOfWork.SupplierRepository.GetSuppliersCollectionAsync(searchText ?? string.Empty, pageIndex, pageSize);

                return Results.Ok(paginatedResult);
            });

            // GET handler method for returning single SupplierDto object
            app.MapGet("api/suppliers/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke SupplierRepository's method for returning
                // single SupplierDto object
                SupplierDto? supplierDto = await unitOfWork.SupplierRepository.GetSingleSupplierAsync(id);

                // If supplierDto is not null, then return OK result
                // along with supplierDto.
                // Otherwise return NotFound (404) result
                return supplierDto != null ? Results.Ok(supplierDto) : Results.NotFound();
            });

            // POST handler method for creating new Supplier
            app.MapPost("api/suppliers/create", async ([FromServices] IUnitOfWork unitOfWork, SupplierDto supplierDto) =>
            {
                // Validate supplierDto using SupplierRepository's
                // method ValidateSupplierAsync
                var errorCheck = await unitOfWork.SupplierRepository.ValidateSupplierAsync(supplierDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke SupplierRepository's method for creating new Supplier
                    await unitOfWork.SupplierRepository.CreateNewSupplierAsync(supplierDto);

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

            // PATCH handler method for editing selected Supplier
            app.MapPatch("api/suppliers/patch", async ([FromServices] IUnitOfWork unitOfWork, SupplierDto supplierDto) =>
            {
                // Validate supplierDto using SupplierRepository's
                // method ValidateSupplierAsync
                var errorCheck = await unitOfWork.SupplierRepository.ValidateSupplierAsync(supplierDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke SupplierRepository's method for editing selected Supplier
                    await unitOfWork.SupplierRepository.EditSupplierAsync(supplierDto);

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

            // DELETE handler method for deleting selected Supplier
            app.MapDelete("api/suppliers/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke SupplierRepository's method for deleting selected Supplier
                    await unitOfWork.SupplierRepository.DeleteSupplierAsync(id);

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

            // GET handler method for returning list of all Supplier records
            app.MapGet("api/suppliers/all", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke SupplierRepository's method for returning all Supplier records
                var response = await unitOfWork.SupplierRepository.GetAllSuppliersAsync();

                return Results.Ok(response);
            });
        }
    }
}
