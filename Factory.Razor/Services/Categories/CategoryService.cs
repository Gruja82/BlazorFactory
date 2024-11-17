using Factory.Shared;
using Microsoft.AspNetCore.Http.Extensions;

namespace Factory.Razor.Services.Categories
{
    // Implementation class for ICategoryService
    public class CategoryService:ICategoryService
    {
        private readonly HttpClient client;

        public CategoryService(HttpClient client)
        {
            this.client = client;
        }

        // Create new Category
        public async Task<object> CreateNewCategoryAsync(CategoryDto categoryDto)
        {
            // Invoke API method for creating new Category
            var response = await client.PostAsJsonAsync<CategoryDto>("api/categories/create", categoryDto);

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

        // Delete selected Category
        public async Task<int> DeleteCategoryAsync(int id)
        {
            // Invoke API method for deleting selected Category
            var response = await client.DeleteAsync($"api/categories/delete/{id}");

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

        // Edit selected Category
        public async Task<object> EditCategoryAsync(CategoryDto categoryDto)
        {
            // Invoke API method for creating new Category
            var response = await client.PatchAsJsonAsync<CategoryDto>("api/categories/patch", categoryDto);

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

        // Return paginated filtered list of CategoryDto objects
        public async Task<object> GetCategoriesAsync(string? searchText, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API URL
            string baseUrl = "api/categories";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            // Invoke API method for returning paginated filtered
            // list of CategoryDto objects
            var response = await client.GetAsync(fullUrl);

            // If response is not null
            if (response != null)
            {
                // If returned status code marks success
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    var categories = await response.Content.ReadFromJsonAsync<Pagination<CategoryDto>>();

                    // If categories is not null then return
                    // categories, otherwise return new 
                    // Pagination<CategoryDto>
                    return categories ?? new Pagination<CategoryDto>();
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

        // Return single CategoryDto object
        public async Task<object> GetSingleCategoryAsync(int id)
        {
            // Invoke API method for returning single CategoryDto object
            var response = await client.GetAsync($"api/categories/{id}");

            // If response is not null
            if (response != null)
            {
                // If returned status code marks success
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    CategoryDto? categoryDto = await response.Content.ReadFromJsonAsync<CategoryDto>();

                    // If categoryDto is not null, return categoryDto
                    // otherwise return 404
                    return categoryDto ?? new CategoryDto();
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

        // Return all Categories
        public async Task<object> GetAllCategoriesAsync()
        {
            // Invoke API method for returning
            // list of all Categories
            var response = await client.GetAsync("api/categories/all");

            // If response is not null
            if (response != null)
            {
                // If returned status code marks success
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();

                    // If categories is not null then return
                    // categories, otherwise return new 
                    // List<CategoryDto>
                    return categories ?? new List<CategoryDto>();
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
