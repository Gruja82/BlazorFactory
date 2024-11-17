using Factory.Api.Repositories.UoW;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Factory.Api.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Order entity
    public static class OrderModule
    {
        public static void MapOrderEndpoint(this WebApplication app)
        {
            // GET handler method for returning paginated list of OrderDto objects
            app.MapGet("api/orders", async ([FromServices] IUnitOfWork unitOfWork, [AsParameters] QueryStringParamsOrder queryStrings) =>
            {
                // Invoke OrderRepository's method for returning
                // collection of OrderDto objects
                var paginatedResult = await unitOfWork.OrderRepository.GetOrdersCollectionAsync(queryStrings.SearchText ?? string.Empty, queryStrings.StringDate ?? string.Empty,
                    queryStrings.Customer ?? string.Empty, queryStrings.PageIndex, queryStrings.PageSize);

                return Results.Ok(paginatedResult);
            });

            // GET handler method for returning single OrderDto object
            app.MapGet("api/orders/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke OrderRepository's method for returning
                // single OrderDto object
                OrderDto? orderDto = await unitOfWork.OrderRepository.GetSingleOrderAsync(id);

                // If orderDto is not null, then return OK result
                // along with orderDto.
                // Otherwise return NotFound (404) result
                return orderDto != null ? Results.Ok(orderDto) : Results.NotFound();
            });

            // POST handler method for creating new Order
            app.MapPost("api/orders/create", async ([FromServices] IUnitOfWork unitOfWork, OrderDto orderDto) =>
            {
                // Validate orderDto using OrderRepository's
                // method ValidateOrderAsync
                var errorCheck = await unitOfWork.OrderRepository.ValidateOrderAsync(orderDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke OrderRepository's method for creating new Order
                    await unitOfWork.OrderRepository.CreateNewOrderAsync(orderDto);

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

            // PATCH handler method for editing selected Order
            app.MapPatch("api/orders/patch", async ([FromServices] IUnitOfWork unitOfWork, OrderDto orderDto) =>
            {
                // Validate orderDto using OrderRepository's
                // method ValidateOrderAsync
                var errorCheck = await unitOfWork.OrderRepository.ValidateOrderAsync(orderDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke OrderRepository's method for editing selected Order
                    await unitOfWork.OrderRepository.EditOrderAsync(orderDto);

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

            // DELETE handler method for deleting selected Order
            app.MapDelete("api/orders/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke OrderRepository's method for deleting selected Order
                    await unitOfWork.OrderRepository.DeleteOrderAsync(id);

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

            // GET handler method for returning list of all Order dates
            app.MapGet("api/orders/dates", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke OrderRepository's method for returning
                // list of Order dates
                var orderDates = await unitOfWork.OrderRepository.GetAllOrderDatesAsync();

                return Results.Ok(orderDates);
            });
        }
    }
}
