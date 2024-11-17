using Factory.Shared;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http.Json;

namespace Factory.Blazor.Services.Categories
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
            try
            {
                // Invoke API method for creating new Category
                var response = await client.PostAsJsonAsync<CategoryDto>("api/categories/create", categoryDto);

                // If returned result is not null
                if (response != null)
                {
                    // If returned status code is 201 Created,
                    // then return simple string
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
                // Otherwise return simple error string message
                else
                {
                    return "Unexpected error occured!";
                }
            }
            catch (HttpRequestException ex)
            {
                return $"There was a problem when trying to save this category to database. {ex.StatusCode}";
            }
        }

        // Delete selected Category
        public async Task<object> DeleteCategoryAsync(int id)
        {
            try
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

                return $"There was a problem when trying to delete this category from database. {ex.StatusCode}";
            }
        }

        // Edit selected Category
        public async Task<object> EditCategoryAsync(CategoryDto categoryDto)
        {
            try
            {
                // Invoke API method for editing selected Category
                var response = await client.PatchAsJsonAsync<CategoryDto>("api/categories/patch", categoryDto);

                // If returned result is not null,
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
                return $"There was a problem when trying to save this category to database. {ex.StatusCode}";
            }
        }

        // Return paginated filtered list of CategoryDto objects
        public async Task<object> GetCategoriesAsync(string? searchText, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/categories";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            try
            {
                // Invoke API method for returning paginated filtered
                // list of CategoryDto objects
                var response = await client.GetAsync(fullUrl);

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks Success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the response
                        var categories = await response.Content.ReadFromJsonAsync<Pagination<CategoryDto>>();

                        // If categories is null, then return new
                        // Pagination<CategoryDto>, otherwise
                        // return categories
                        return categories ?? new Pagination<CategoryDto>();
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
                return $"There was a problem when trying to load list of categories. {ex.StatusCode}";
            }
            
        }

        // Return single CategoryDto object
        public async Task<object> GetSingleCategoryAsync(int id)
        {
            try
            {
                // Invoke API method for returning single CategoryDto object
                var response = await client.GetAsync($"api/categories/{id}");

                // If response is not null
                if (response != null)
                {
                    // If returned status code is Success status code
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the result
                        CategoryDto? categoryDto = await response.Content.ReadFromJsonAsync<CategoryDto>();

                        // If categoryDto is not null, return categorytDto
                        // Otherwise return new CategoryDto object
                        return categoryDto ?? new CategoryDto();
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
                return $"There was a problem when trying to load requsted category. {ex.StatusCode}";
            }
        }

        public async Task<object> GetAllCategoriesAsync()
        {
            try
            {
                // Invoke API method for returning list of all Categories
                var response = await client.GetAsync("api/categories/all");

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the result
                        List<CategoryDto>? categoryDtos = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();

                        // If categoryDtoa is not null, return categorytDtoa
                        // Otherwise return new List<CategoryDto> object
                        return categoryDtos ?? new List<CategoryDto>();
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
                return $"There was a problem when trying to load requsted category. {ex.StatusCode}";
            }
        }
    }
}
