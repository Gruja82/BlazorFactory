
using Factory.Shared;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http.Json;

namespace Factory.Blazor.Services.Orders
{
    // Implementation class for IOrderService
    public class OrderService:IOrderService
    {
        private readonly HttpClient client;

        public OrderService(HttpClient client)
        {
            this.client = client;
        }

        // Create new Order
        public async Task<object> CreateNewOrderAsync(OrderDto orderDto)
        {
            try
            {
                // Invoke API method for creating new Order
                var response = await client.PostAsJsonAsync<OrderDto>("api/orders/create", orderDto);

                // If returned result is not null
                if (response != null)
                {
                    // If returned status code is 201 Created
                    // then return simple string
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        return "Created";
                    }
                    // Otherwise return Dictionary with errors
                    else
                    {
                        var errors = await response.Content.ReadFromJsonAsync<IDictionary<string, string>>();
                        return errors ?? new Dictionary<string, string>();
                    }
                }
                // Otherwise return simple error string message
                else
                {
                    return "Unexpected error occured!";
                }
            }
            catch (HttpRequestException ex)
            {
                return $"There was a problem when trying to save this order to database. {ex.StatusCode}";
            }
        }

        // Delete selected Order
        public async Task<object> DeleteOrderAsync(int id)
        {
            try
            {
                // Invoke API method for deleting selected Order
                var response = await client.DeleteAsync($"api/orders/delete/{id}");

                // If returned result is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Return status code 204 No Content
                        return System.Net.HttpStatusCode.NoContent;
                    }
                    // Otherwise return status code 400 Bad Request
                    else
                    {
                        return System.Net.HttpStatusCode.BadRequest;
                    }
                }
                // Otherwise return simple error string message
                else
                {
                    return "Unexpected error occured!";
                }
            }
            catch (HttpRequestException ex)
            {
                return $"There was a problem when trying to delete this order from database. {ex.StatusCode}";
            }
        }

        // Edit selected Order
        public async Task<object> EditOrderAsync(OrderDto orderDto)
        {
            try
            {
                // Invoke API method for editing selected Order
                var response = await client.PatchAsJsonAsync<OrderDto>("api/orders/patch", orderDto);

                // If returned result is not null
                if (response != null)
                {
                    // If returned status code is 204 No Content
                    // then return simple string
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return "Edited";
                    }
                    // Otherwise return Dictionary containing errors
                    else
                    {
                        var errors = await response.Content.ReadFromJsonAsync<IDictionary<string, string>>();
                        return errors ?? new Dictionary<string, string>();
                    }
                }
                // Otherwise return status code 400 Bad Request
                else
                {
                    return System.Net.HttpStatusCode.BadRequest;
                }
            }
            catch (HttpRequestException ex)
            {
                return $"There was a problem when trying to edit this order. {ex.StatusCode}";
            }
        }

        // Return paginated filtered list of OrderDto objects
        public async Task<object> GetOrdersAsync(string? searchText, string? stringDate, string? customer, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["stringDate"] = stringDate ?? string.Empty;
            queryParams["customer"] = customer ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/orders";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            try
            {
                // Invoke API method for returning paginated filtered
                // list of OrderDto objects
                var response = await client.GetAsync(fullUrl);

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the response
                        var orders = await response.Content.ReadFromJsonAsync<Pagination<OrderDto>>();

                        // If orders is null, then return new
                        // Pagination<OrderDto>, otherwise
                        // return orders
                        return orders ?? new Pagination<OrderDto>();
                    }
                    // Otherwise return status code 404 Not Found
                    else
                    {
                        return System.Net.HttpStatusCode.NotFound;
                    }
                }
                // Otherwise return status code 400 Bad Request
                else
                {
                    return System.Net.HttpStatusCode.BadRequest;
                }
            }
            catch (HttpRequestException ex)
            {
                return $"There was a problem when trying to load list of orders. {ex.StatusCode}";
            }
        }

        // Return single OrderDto object
        public async Task<object> GetSingleOrderAsync(int id)
        {
            try
            {
                // Invoke API method for returning single OrderDto object
                var response = await client.GetAsync($"api/orders/{id}");

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the result
                        OrderDto? orderDto = await response.Content.ReadFromJsonAsync<OrderDto>();

                        // If orderDto is not null, return orderDto
                        // Otherwise return new OrderDto object
                        return orderDto ?? new OrderDto();
                    }
                    // Otherwise return status code 404 Not Found
                    else
                    {
                        return System.Net.HttpStatusCode.NotFound;
                    }
                }
                // Otherwise return status code 400 Bad Request
                else
                {
                    return System.Net.HttpStatusCode.BadRequest;
                }
            }
            catch (HttpRequestException ex)
            {
                return $"There was a problem when trying to load requsted order. {ex.StatusCode}";
            }
        }

        // Return all Order dates
        public async Task<object> ReturnOrderDatesAsync()
        {
            try
            {
                // Invoke API method for returning list of Order dates
                var response = await client.GetAsync("api/orders/dates");

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the result
                        List<string>? orderDates = await response.Content.ReadFromJsonAsync<List<string>>();

                        // If orderDates is not null, return orderDates
                        // Otherwise return new List<string>
                        return orderDates ?? new List<string>();
                    }
                    // Otherwise return status code 404 Not Found
                    else
                    {
                        return System.Net.HttpStatusCode.NotFound;
                    }
                }
                // Otherwise return status code 400 Bad Request
                else
                {
                    return System.Net.HttpStatusCode.BadRequest;
                }
            }
            catch (HttpRequestException ex)
            {
                return $"There was a problem when trying to load requsted list of order dates. {ex.StatusCode}";
            }
        }
    }
}
