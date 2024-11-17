using Factory.Shared;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http.Json;

namespace Factory.Blazor.Services.Products
{
    // Implementation class for IPRoductService
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
            try
            {
                // Invoke API method for creating new Product
                var response = await client.PostAsJsonAsync<ProductDto>("api/products/create", productDto);

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
                return $"There was a problem when trying to save this product to database. {ex.StatusCode}";
            }
        }

        // Delete selected Product
        public async Task<object> DeleteProductAsync(int id)
        {
            try
            {
                // Invoke API method for deleting selected Product
                var response = await client.DeleteAsync($"api/products/delete/{id}");

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
                return $"There was a problem when trying to delete this product from database. {ex.StatusCode}";
            }
        }

        // Edit selected Product
        public async Task<object> EditProductAsync(ProductDto productDto)
        {
            try
            {
                // Invoke API method for editing selected Product
                var response = await client.PatchAsJsonAsync<ProductDto>("api/products/patch", productDto);

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
                return $"There was a problem when trying to edit this product. {ex.StatusCode}";
            }
        }

        // Return all Products
        public async Task<object> GetAllProductsAsync()
        {
            try
            {
                // Invoke API method for returning list of all Products
                var response = await client.GetAsync("api/products/all");

                // If response is not null
                if (response != null)
                {
                    // If returned result marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Return the content of the response
                        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();

                        // If products is null, then return new
                        // List<ProductDto>, otherwise
                        // return products
                        return products ?? new List<ProductDto>();
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
                return $"There was a problem when trying to load list of products. {ex.StatusCode}";
            }
        }

        // Return paginated filtered list of ProductDto objects
        public async Task<object> GetProductsAsync(string? searchText, string? category, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["category"] = category ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/products";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            try
            {
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

                        // If products is null, then return new
                        // Pagination<ProductDto>, otherwise
                        // return products
                        return products ?? new Pagination<ProductDto>();
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
                return $"There was a problem when trying to load list of products. {ex.StatusCode}";
            }
        }

        // Return single ProductDto object
        public async Task<object> GetSingleProductAsync(int id)
        {
            try
            {
                // Invoke API method for returning single ProductDto object
                var response = await client.GetAsync($"api/products/{id}");

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the result
                        ProductDto? productDto = await response.Content.ReadFromJsonAsync<ProductDto>();

                        // If productDto is not null, return productDto
                        // Otherwise return new ProductDto object
                        return productDto ?? new ProductDto();
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
                return $"There was a problem when trying to load requsted product. {ex.StatusCode}";
            }
        }
    }
}
