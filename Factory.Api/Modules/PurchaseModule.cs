using Factory.Api.Repositories.UoW;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Factory.Api.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Purchase entity
    public static class PurchaseModule
    {
        public static void MapPurchaseEndpoint(this WebApplication app)
        {
            // GET handler method for returning paginated filtered list of PurchaseDto objects
            app.MapGet("api/purchases", async ([FromServices] IUnitOfWork unitOfWork, [AsParameters] QueryStringParamsPurchase queryStrings) =>
            {
                // Invoke PurchaseRepository's method for returning
                // collection of PurchaseDto objects
                var paginatedResult = await unitOfWork.PurchaseRepository.GetPurchasesCollectionAsync(queryStrings.SearchText ?? string.Empty, queryStrings.StringDate ?? string.Empty,
                    queryStrings.Supplier ?? string.Empty, queryStrings.PageIndex, queryStrings.PageSize);

                return Results.Ok(paginatedResult);
            });

            // GET handler method for returning single PurchaseDto object
            app.MapGet("api/purchases/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke PurchaseRepository's method for returning
                // single PurchaseDto object
                PurchaseDto? purchaseDto = await unitOfWork.PurchaseRepository.GetSinglePurchaseAsync(id);

                // If purchaseDto is not null, then return OK result
                // along with purchaseDto.
                // Otherwise return NotFound (404) result
                return purchaseDto != null ? Results.Ok(purchaseDto) : Results.NotFound();
            });

            // POST handler method for creating new Purchase
            app.MapPost("api/purchases/create", async ([FromServices] IUnitOfWork unitOfWork, PurchaseDto purchaseDto) =>
            {
                // Validate purchaseDto using PurchaseRepository's
                // method ValidatePurchaseAsync
                var errorCheck = await unitOfWork.PurchaseRepository.ValidatePurchaseAsync(purchaseDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke PurchaseRepository's method for creating new Purchase
                    await unitOfWork.PurchaseRepository.CreateNewPurchaseAsync(purchaseDto);

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

            // PATCH handler method for editing selected Purchase
            app.MapPatch("api/purchases/patch", async ([FromServices] IUnitOfWork unitOfWork, PurchaseDto purchaseDto) =>
            {
                // Validate purchaseDto using PurchaseRepository's
                // method ValidatePurchaseAsync
                var errorCheck = await unitOfWork.PurchaseRepository.ValidatePurchaseAsync(purchaseDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke PurchaseRepository's method for editing selected Purchase
                    await unitOfWork.PurchaseRepository.EditPurchaseAsync(purchaseDto);

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

            // DELETE handler method for deleting selected Purchase
            app.MapDelete("api/purchases/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke PurchaseRepository's method for deleting selected Purchase
                    await unitOfWork.PurchaseRepository.DeletePurchaseAsync(id);

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

            // GET handler method for returning list of all Purchase dates
            app.MapGet("api/purchases/dates", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke PurchaseRepository's method for returning
                // list of Purchase dates
                var purchaseDates = await unitOfWork.PurchaseRepository.GetAllPurchaseDatesAsync();

                return Results.Ok(purchaseDates);
            });
        }
    }
}
