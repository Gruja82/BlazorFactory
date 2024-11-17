using Factory.Shared;
using Microsoft.AspNetCore.Http.Extensions;

namespace Factory.Razor.Services.Materials
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
            // Invoke API method for creating new Material
            var response = await client.PostAsJsonAsync<MaterialDto>("api/materials/create", materialDto);

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

        // Delete selected Material
        public async Task<int> DeleteMaterialAsync(int id)
        {
            // Invoke API method for deleting selected Material
            var response = await client.DeleteAsync($"api/materials/delete/{id}");

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

        // Edit selected Material
        public async Task<object> EditMaterialAsync(MaterialDto materialDto)
        {
            // Invoke API method for editing selected Material
            var response = await client.PatchAsJsonAsync<MaterialDto>("api/materials/patch", materialDto);

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

        // Return paginated filtered list of MaterialDto objects
        public async Task<object> GetMaterialsAsync(string? searchText, string? category, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["category"] = category ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API URL
            string baseUrl = "api/materials";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

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

                    // If materials is not null then return
                    // materials, otherwise return new 
                    // Pagination<MaterialDto>
                    return materials ?? new Pagination<MaterialDto>();
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

        // Return single MaterialDto object
        public async Task<object> GetSingleMaterialAsync(int id)
        {
            // Invoke API method for returning single MaterialDto object
            var response = await client.GetAsync($"api/materials/{id}");

            // If response is not null
            if (response != null)
            {
                // If returned status code marks success
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    MaterialDto? materialDto = await response.Content.ReadFromJsonAsync<MaterialDto>();

                    // If materialDto is not null, return materialDto
                    // otherwise return new MaterialDto
                    return materialDto ?? new MaterialDto();
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

        // Return all Materials
        public async Task<object> GetAllMaterialsAsync()
        {
            // Invoke API method for returning
            // list of all Materials
            var response = await client.GetAsync("api/materials/all");

            // If response is not null
            if (response != null)
            {
                // If returned status code marks success
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    var materials = await response.Content.ReadFromJsonAsync<List<MaterialDto>>();

                    // If materials is not null then return
                    // materials, otherwise return new 
                    // List<MaterialDto>
                    return materials ?? new List<MaterialDto>();
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
