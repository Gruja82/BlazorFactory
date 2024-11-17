using Factory.Shared;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http.Json;

namespace Factory.Blazor.Services.Materials
{
    // Implementation class for IMaterialService
    public class MaterialService:IMaterialService
    {
        private readonly HttpClient client;

        public MaterialService(HttpClient client)
        {
            this.client = client;
        }

        // Create new Material
        public async Task<object> CreateNewMaterialAsync(MaterialDto materialDto)
        {
            try
            {
                // Invoke API method for creating new Material
                var response = await client.PostAsJsonAsync<MaterialDto>("api/materials/create", materialDto);

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
                return $"There was a problem when trying to save this material to database. {ex.StatusCode}";
            }
        }

        // Delete selected Material
        public async Task<object> DeleteMaterialAsync(int id)
        {
            try
            {
                // Invoke API method for deleting selected Material
                var response = await client.DeleteAsync($"api/materials/delete/{id}");

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
                return $"There was a problem when trying to delete this material from database. {ex.StatusCode}";
            }
        }

        // Edit selected Material
        public async Task<object> EditMaterialAsync(MaterialDto materialDto)
        {
            try
            {
                // Invoke API method for editing selected Material
                var response = await client.PatchAsJsonAsync<MaterialDto>("api/materials/patch", materialDto);

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
                return $"There was a problem when trying to edit this material. {ex.StatusCode}";
            }
        }

        // Return all Materials
        public async Task<object> GetAllMaterialsAsync()
        {
            try
            {
                // Invoke API method for retreiving all Material records
                var response = await client.GetAsync("api/materials/all");

                // If response is not null
                if (response != null)
                {
                    // If returned result marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Return the content of the response
                        var materials = await response.Content.ReadFromJsonAsync<List<MaterialDto>>();

                        // If materials is null, then return new
                        // List<MaterialDto>, otherwise
                        // return materials
                        return materials ?? new List<MaterialDto>();
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
                return $"There was a problem when trying to load list of materials. {ex.StatusCode}";
            }
        }

        // Return paginated filtered list of MaterialDto objects
        public async Task<object> GetMaterialsAsync(string? searchText, string? category, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["category"] = category ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/materials";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            try
            {
                // Invoke API method for returning paginated filtered
                // list of MaterialDto objects
                var response = await client.GetAsync(fullUrl);

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the response
                        var materials = await response.Content.ReadFromJsonAsync<Pagination<MaterialDto>>();

                        // If materials is null, then return new
                        // Pagination<MaterialDto>, otherwise
                        // return materials
                        return materials ?? new Pagination<MaterialDto>();
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
                return $"There was a problem when trying to load list of materials. {ex.StatusCode}";
            }
        }

        // Return single MaterialDto object
        public async Task<object> GetSingleMaterialAsync(int id)
        {
            try
            {
                // Invoke API method for returning single MaterialDto object
                var response = await client.GetAsync($"api/materials/{id}");

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the result
                        MaterialDto? materialDto = await response.Content.ReadFromJsonAsync<MaterialDto>();

                        // If materialDto is not null, return materialDto
                        // Otherwise return new MaterialDto object
                        return materialDto ?? new MaterialDto();
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
                return $"There was a problem when trying to load requsted material. {ex.StatusCode}";
            }
        }
    }
}
