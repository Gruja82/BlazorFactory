using Factory.Shared;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http.Json;

namespace Factory.Blazor.Services.Suppliers
{
    // Implementation class for ISupplierService
    public class SupplierService:ISupplierService
    {
        private readonly HttpClient client;

        public SupplierService(HttpClient client)
        {
            this.client = client;
        }

        // Create new Supplier
        public async Task<object> CreateNewSupplierAsync(SupplierDto supplierDto)
        {
            try
            {
                // Invoke API method for creating new Supplier
                var response = await client.PostAsJsonAsync<SupplierDto>("api/suppliers/create", supplierDto);

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
                return $"There was a problem when trying to save this supplie to database. {ex.StatusCode}";
            }
        }

        // Delete selected Supplier
        public async Task<object> DeleteSupplierAsync(int id)
        {
            try
            {
                // Invoke API method for deleting selected Supplier
                var response = await client.DeleteAsync($"api/suppliers/delete/{id}");

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
                return $"There was a problem when trying to delete this supplier from database. {ex.StatusCode}";
            }
        }

        // Edit selected Supplier
        public async Task<object> EditSupplierAsync(SupplierDto supplierDto)
        {
            try
            {
                // Invoke API method for editing selected Supplier
                var response = await client.PatchAsJsonAsync<SupplierDto>("api/suppliers/patch", supplierDto);

                // If returned result is not null
                if (response != null)
                {
                    // If returned status code is 204 No Content,
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
                return $"There was a problem when trying to edit this supplier. {ex.StatusCode}";
            }
        }

        // Return all Suppliers
        public async Task<object> GetAllSuppliersAsync()
        {
            try
            {
                // Invoke API method for returning all Suppliers
                var response = await client.GetAsync("api/suppliers/all");

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the result
                        List<SupplierDto>? supplierDtos = await response.Content.ReadFromJsonAsync<List<SupplierDto>>();

                        // If supplierDtos is not null, return supplierDtos
                        // Otherwise return new List<SupplierDto> object
                        return supplierDtos ?? new List<SupplierDto>();
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
                return $"There was a problem when trying to load list of suppliers. {ex.StatusCode}";
            }
        }

        // Return single SupplierDto object
        public async Task<object> GetSingleSupplierAsync(int id)
        {
            try
            {
                // Invoke API method for returning single SupplierDto object
                var response = await client.GetAsync($"api/suppliers/{id}");

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the result
                        SupplierDto? supplierDto = await response.Content.ReadFromJsonAsync<SupplierDto>();

                        // If supplierDto is not null, return supplierDto
                        // Otherwise return new SupplierDto object
                        return supplierDto ?? new SupplierDto();
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
                return $"There was a problem when trying to load requsted supplier. {ex.StatusCode}";
            }
        }

        // Return paginated filtered list of SupplierDto objects
        public async Task<object> GetSuppliersAsync(string? searchText, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/suppliers";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            try
            {
                // Invoke API method for returning paginated filtered
                // list of SupplierDto objects
                var response = await client.GetAsync(fullUrl);

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks Success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the response
                        var suppliers = await response.Content.ReadFromJsonAsync<Pagination<SupplierDto>>();

                        // If suppliers is null, then return new
                        // Pagination<SupplierDto>, otherwise
                        // return suppliers
                        return suppliers ?? new Pagination<SupplierDto>();
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
                return $"There was a problem when trying to load list of suppliers. {ex.StatusCode}";
            }
        }
    }
}
