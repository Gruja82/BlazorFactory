using Factory.Shared;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http.Json;

namespace Factory.Blazor.Services.Productions
{
    // Implementation class for IProductionService
    public class ProductionService:IProductionService
    {
        private readonly HttpClient client;

        public ProductionService(HttpClient client)
        {
            this.client = client;
        }

        // Create new Production
        public async Task<object> CreateNewProductionAsync(ProductionDto productionDto)
        {
            try
            {
                // Invoke API method for creating new Production
                var response = await client.PostAsJsonAsync<ProductionDto>("api/productions/create", productionDto);

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
                return $"There was a problem when trying to save this production to database. {ex.StatusCode}";
            }
        }

        // Delete selected Production
        public async Task<object> DeleteProductionAsync(int id)
        {
            try
            {
                // Invoke API method for deleting selected Production
                var response = await client.DeleteAsync($"api/productions/delete/{id}");

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
                return $"There was a problem when trying to delete this production from database. {ex.StatusCode}";
            }
        }

        // Edit selected Production
        public async Task<object> EditProductionAsync(ProductionDto productionDto)
        {
            try
            {
                // Invoke API method for editing selected Production
                var response = await client.PatchAsJsonAsync<ProductionDto>("api/productions/patch", productionDto);

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
                return $"There was a problem when trying to edit this production. {ex.StatusCode}";
            }
        }

        // Return paginated filtered list of ProductionDto objects
        public async Task<object> GetProductionsAsync(string? searchText, string? stringDate, string? productName, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["stringDate"] = stringDate ?? string.Empty;
            queryParams["productName"] = productName ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/productions";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            try
            {
                // Invoke API method for returning paginated filtered
                // list of ProductionDto objects
                var response = await client.GetAsync(fullUrl);

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the response
                        var productions = await response.Content.ReadFromJsonAsync<Pagination<ProductionDto>>();

                        // If productions is null, then return new
                        // Pagination<ProductionDto>, otherwise
                        // return productions
                        return productions ?? new Pagination<ProductionDto>();
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
                return $"There was a problem when trying to load list of productions. {ex.StatusCode}";
            }
        }

        // Return single ProductionDto object
        public async Task<object> GetSingleProductionAsync(int id)
        {
            try
            {
                // Invoke API method for returning single ProductionDto object
                var response = await client.GetAsync($"api/productions/{id}");

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the result
                        ProductionDto? productionDto = await response.Content.ReadFromJsonAsync<ProductionDto>();

                        // If productionDto is not null, return productionDto
                        // Otherwise return new ProductionDto object
                        return productionDto ?? new ProductionDto();
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
                return $"There was a problem when trying to load requsted production. {ex.StatusCode}";
            }
        }

        // Return all Production dates
        public async Task<object> ReturnProductionDatesAsync()
        {
            try
            {
                // Invoke API method for returning list of Production dates
                var response = await client.GetAsync("api/productions/dates");

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the result
                        List<string>? productionDates = await response.Content.ReadFromJsonAsync<List<string>>();

                        // If productionDates is not null, return productionDates
                        // Otherwise return new List<string>
                        return productionDates ?? new List<string>();
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
                return $"There was a problem when trying to load requsted list of production dates. {ex.StatusCode}";
            }
        }
    }
}
