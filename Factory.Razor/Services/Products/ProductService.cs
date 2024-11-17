using Factory.Shared;
using Microsoft.AspNetCore.Http.Extensions;

namespace Factory.Razor.Services.Products
{
    // Implementation class for IProductService
    public class ProductService:IProductService
    {
        private readonly HttpClient client;

        public ProductService(HttpClient client)
        {
            this.client = client;
        }

        // Create new Product
        public async Task<object> CreateNewProductAsync(ProductDto productDto)
        {
            // Invoke API method for creating new Product
            var response = await client.PostAsJsonAsync<ProductDto>("api/products/create", productDto);

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

        // Delete selected Product
        public async Task<int> DeleteProductAsync(int id)
        {
            // Invoke API method for deleting selected Product
            var response = await client.DeleteAsync($"api/products/delete/{id}");

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

        // Edit selected Product
        public async Task<object> EditProductAsync(ProductDto productDto)
        {
            // Invoke API method for editing selected Product
            var response = await client.PatchAsJsonAsync<ProductDto>("api/products/patch", productDto);

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

        // Return paginated filtered list of ProductDto objects
        public async Task<object> GetProductsAsync(string? searchText, string? category, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["category"] = category ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API URL
            string baseUrl = "api/products";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            // Invoke API method for returning paginated filtered
            // list of ProductDto objects
            var response = await client.GetAsync(fullUrl);

            // If response is not null
            if (response != null)
            {
                // If returned status code marks success
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    var products = await response.Content.ReadFromJsonAsync<Pagination<ProductDto>>();

                    // If products is not null then return
                    // products, otherwise return new 
                    // Pagination<ProductDto>
                    return products ?? new Pagination<ProductDto>();
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

        // Return single ProductDto object
        public async Task<object> GetSingleProductAsync(int id)
        {
            // Invoke API method for returning single ProductDto object
            var response = await client.GetAsync($"api/products/{id}");

            // If response is not null
            if (response != null)
            {
                // If returned status code marks success
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    ProductDto? productDto = await response.Content.ReadFromJsonAsync<ProductDto>();

                    // If productDto is not null, return productDto
                    // otherwise return new ProductDto
                    return productDto ?? new ProductDto();
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
