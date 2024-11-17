using Factory.Shared;
using Microsoft.AspNetCore.Http.Extensions;

namespace Factory.Razor.Services.Customers
{
    // Implementation class for ICustomerService
    public class CustomerService:ICustomerService
    {
        private readonly HttpClient client;

        public CustomerService(HttpClient client)
        {
            this.client = client;
        }

        // Create new Customer
        public async Task<object> CreateNewCustomerAsync(CustomerDto customerDto)
        {
            // Invoke API method for creating new Customer
            var response = await client.PostAsJsonAsync<CustomerDto>("api/customers/create", customerDto);

            // If returned result is not null
            if (response != null)
            {
                // If returned status code is 201
                // Created, then return simple string
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return "Created";
                }
                // Otherwise return Dictionary with errors
                else
                {
                    var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    return errors ?? new Dictionary<string, string>();
                }
            }
            // Otherwise return status code 503 (Service Unavaliable)
            else
            {
                return System.Net.HttpStatusCode.ServiceUnavailable;
            }
        }

        // Delete selected Customer
        public async Task<int> DeleteCustomerAsync(int id)
        {
            // Invoke API method for deleting selected Customer
            var response = await client.DeleteAsync($"api/customers/delete/{id}");

            // If returned result is not null
            if (response != null)
            {
                // If returned status code marks success
                if (response.IsSuccessStatusCode)
                {
                    // Return status code 204 - No Content
                    return StatusCodes.Status204NoContent;
                }
                // Otherwise, return status code 400 Bad Request
                else
                {
                    return StatusCodes.Status400BadRequest;
                }
            }
            // Otherwise return status code 503 (Service Unavaliable)
            else
            {
                return StatusCodes.Status503ServiceUnavailable;
            }
        }

        // Edit selected Customer
        public async Task<object> EditCustomerAsync(CustomerDto customerDto)
        {
            // Invoke API method for editing selected Customer
            var response = await client.PatchAsJsonAsync<CustomerDto>("api/customers/patch", customerDto);

            // If returned result is not null
            if (response != null)
            {
                // If returned status code is 204
                // No Content, then return simple string
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return "Edited";
                }
                // Otherwise return Dictionary with errors
                else
                {
                    var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    return errors ?? new Dictionary<string, string>();
                }
            }
            // Otherwise return status code 503 (Service Unavaliable)
            else
            {
                return System.Net.HttpStatusCode.ServiceUnavailable;
            }
        }

        // Return paginated filtered list of CustomerDto objects
        public async Task<object> GetCustomersAsync(string? searchText, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API URL
            string baseUrl = "api/customers";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            // Invoke API method for returning paginated filtered
            // list of CustomerDto objects
            var response = await client.GetAsync(fullUrl);

            // If response is not null
            if (response != null)
            {
                // If returned status code marks success
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    var customers = await response.Content.ReadFromJsonAsync<Pagination<CustomerDto>>();

                    // If customers is not null then return
                    // customers, otherwise return new 
                    // Pagination<CustomerDto>
                    return customers ?? new Pagination<CustomerDto>();
                }
                // Otherwise return status code 404 Not Found
                else
                {
                    return StatusCodes.Status404NotFound;
                }
            }
            // Otherwise return status code 503 - Service Unavaliable
            else
            {
                return StatusCodes.Status503ServiceUnavailable;
            }
        }

        // Return single CustomerDto object
        public async Task<object> GetSingleCustomerAsync(int id)
        {
            // Invoke API method for returning single CustomerDto object
            var response = await client.GetAsync($"api/customers/{id}");

            // If response is not null
            if (response != null)
            {
                // If returned status code marks success
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    CustomerDto? customerDto = await response.Content.ReadFromJsonAsync<CustomerDto>();

                    // If customerDto is not null, return customerDto
                    // otherwise return new MaterialDto
                    return customerDto ?? new CustomerDto();
                }
                // Otherwise return status code 404 Not Found
                else
                {
                    return StatusCodes.Status404NotFound;
                }
            }
            // Otherwise return status code 503 - Service Unavaliable
            else
            {
                return StatusCodes.Status503ServiceUnavailable;
            }
        }
    }
}
