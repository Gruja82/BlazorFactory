using Factory.Api.Repositories.UoW;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Factory.Api.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Customer entity
    public static class CustomerModule
    {
        public static void MapCustomerEndpoint(this WebApplication app)
        {
            // GET handler method for returning paginated list of CustomerDto objects
            app.MapGet("api/customers", async ([FromServices] IUnitOfWork unitOfWork, [FromQuery] string? searchText, [FromQuery] int pageIndex, [FromQuery] int pageSize) =>
            {
                // Invoke CustomerRepository's method for returning
                // collection of CustomerDto objects
                var paginatedResult = await unitOfWork.CustomerRepository.GetCustomersCollectionAsync(searchText ?? string.Empty, pageIndex, pageSize);

                return Results.Ok(paginatedResult);
            });

            // GET handler method for returning single CustomerDto object
            app.MapGet("api/customers/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke CustomerRepository's method for returning
                // single CustomerDto object
                CustomerDto? customerDto = await unitOfWork.CustomerRepository.GetSingleCustomerAsync(id);

                // If customerDto is not null, then return OK result
                // along with customerDto.
                // Otherwise return NotFound (404) result
                return customerDto != null ? Results.Ok(customerDto) : Results.NotFound();
            });

            // POST handler method for creating new Customer
            app.MapPost("api/customers/create", async ([FromServices] IUnitOfWork unitOfWork, CustomerDto customerDto) =>
            {
                // Validate customerDto using CustomerRepository's
                // method ValidateCustomerAsync
                var errorCheck = await unitOfWork.CustomerRepository.ValidateCustomerAsync(customerDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke CustomerRepository's method for creating new Customer
                    await unitOfWork.CustomerRepository.CreateNewCustomerAsync(customerDto);

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

            // PATCH method for editing selected Customer
            app.MapPatch("api/customers/patch", async ([FromServices] IUnitOfWork unitOfWork, CustomerDto customerDto) =>
            {
                // Validate customerDto using CustomerRepository's
                // method ValidateCustomerAsync
                var errorCheck = await unitOfWork.CustomerRepository.ValidateCustomerAsync(customerDto);

                // If there are any errors, then return BadRequest status code
                // along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke CustomerRepository's method for editing selected Customer
                    await unitOfWork.CustomerRepository.EditCustomerAsync(customerDto);

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

            // DELETE handler method for deleting selected Customer
            app.MapDelete("api/customers/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke CustomerRepository's method for deleting selected Customer
                    await unitOfWork.CustomerRepository.DeleteCustomerAsync(id);

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

            // GET handler method for returning list of all Customer records
            app.MapGet("api/customers/all", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke CustomerRepository's method for returning all Customer records
                var response = await unitOfWork.CustomerRepository.GetAllCustomersAsync();

                return Results.Ok(response);
            });
        }
    }
}
