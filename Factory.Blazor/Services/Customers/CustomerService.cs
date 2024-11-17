using Factory.Shared;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http.Json;

namespace Factory.Blazor.Services.Customers
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
            try
            {
                // Invoke API method for creating new Customer
                var response = await client.PostAsJsonAsync<CustomerDto>("api/customers/create", customerDto);

                // If returned result is not null
                if (response != null)
                {
                    // If returned status code is 201 created
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
                return $"There was a problem when trying to save this customer to database. {ex.StatusCode}";
            }
        }

        // Delete selected Customer
        public async Task<object> DeleteCustomerAsync(int id)
        {
            try
            {
                // Invoke API method for deleting selected Customer
                var response = await client.DeleteAsync($"api/customers/delete/{id}");

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
                return $"There was a problem when trying to delete this customer from database. {ex.StatusCode}";
            }
        }

        // Edit selected Customer
        public async Task<object> EditCustomerAsync(CustomerDto customerDto)
        {
            try
            {
                // Invoke API method for editing selected Customer
                var response = await client.PatchAsJsonAsync<CustomerDto>("api/customers/patch", customerDto);

                // If returned result is not null
                if (response != null)
                {
                    // If returned status code is 204 No Content,
                    // then return simple string
                    if (response.StatusCode== System.Net.HttpStatusCode.NoContent)
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
                return $"There was a problem when trying to edit this customer. {ex.StatusCode}";
            }
        }

        // Return paginated filtered list of CustomerDto objects
        public async Task<object> GetCustomersAsync(string? searchText, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/customers";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            try
            {
                // Invoke API method for returning paginated filtered
                // list of CustomerDto objects
                var response = await client.GetAsync(fullUrl);

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks Success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the response
                        var customers = await response.Content.ReadFromJsonAsync<Pagination<CustomerDto>>();

                        // If customers is null, then return new
                        // Pagination<CustomerDto>, otherwise
                        // return customers
                        return customers ?? new Pagination<CustomerDto>();
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
                return $"There was a problem when trying to load list of customers. {ex.StatusCode}";
            }
        }

        // Return single CustomerDto object
        public async Task<object> GetSingleCustomerAsync(int id)
        {
            try
            {
                // Invoke API method for returning single CustomerDto object
                var response = await client.GetAsync($"api/customers/{id}");

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the result
                        CustomerDto? customerDto = await response.Content.ReadFromJsonAsync<CustomerDto>();

                        // If customerDto is not null, return customerDto
                        // Otherwise return new CustomerDto object
                        return customerDto ?? new CustomerDto();
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
                return $"There was a problem when trying to load requsted customer. {ex.StatusCode}";
            }
        }

        // Return all Customers
        public async Task<object> GetAllCustomersAsync()
        {
            try
            {
                // Invoke API method for returning all Customers
                var response = await client.GetAsync("api/customers/all");

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the result
                        List<CustomerDto>? customerDtos = await response.Content.ReadFromJsonAsync<List<CustomerDto>>();

                        // If customerDtos is not null, return customerDtos
                        // Otherwise return new List<CustomerDto> object
                        return customerDtos ?? new List<CustomerDto>();
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
                return $"There was a problem when trying to load list of customers. {ex.StatusCode}";
            }
        }
    }
}
