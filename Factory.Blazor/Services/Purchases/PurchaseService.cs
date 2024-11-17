using Factory.Shared;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http.Json;

namespace Factory.Blazor.Services.Purchases
{
    // Implementation class for IPurchaseService
    public class PurchaseService:IPurchaseService
    {
        private readonly HttpClient client;

        public PurchaseService(HttpClient client)
        {
            this.client = client;
        }

        // Create new Purchase
        public async Task<object> CreateNewPurchaseAsync(PurchaseDto purchaseDto)
        {
            try
            {
                // Invoke API method for creating new Purchase
                var response = await client.PostAsJsonAsync<PurchaseDto>("api/purchases/create", purchaseDto);

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
                return $"There was a problem when trying to save this purchase to database. {ex.StatusCode}";
            }
        }

        // Delete selected Purchase
        public async Task<object> DeletePurchaseAsync(int id)
        {
            try
            {
                // Invoke API method for deleting selected Purchase
                var response = await client.DeleteAsync($"api/purchases/delete/{id}");

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
                return $"There was a problem when trying to delete this purchase from database. {ex.StatusCode}";
            }
        }

        // Edit selected Purchase
        public async Task<object> EditPurchaseAsync(PurchaseDto purchaseDto)
        {
            try
            {
                // Invoke API method for editing selected Purchase
                var response = await client.PatchAsJsonAsync<PurchaseDto>("api/purchases/patch", purchaseDto);

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
                return $"There was a problem when trying to edit this purchase. {ex.StatusCode}";
            }
        }

        // Return paginated filtered list of PurchaseDto objects
        public async Task<object> GetPurchasesAsync(string? searchText, string? stringDate, string? supplier, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["stringDate"] = stringDate ?? string.Empty;
            queryParams["supplier"] = supplier ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/purchases";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            try
            {
                // Invoke API method for returning paginated filtered
                // list of PurchaseDto objects
                var response = await client.GetAsync(fullUrl);

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the response
                        var purchases = await response.Content.ReadFromJsonAsync<Pagination<PurchaseDto>>();

                        // If purchases is null, then return new
                        // Pagination<PurchaseDto>, otherwise
                        // return purchases
                        return purchases ?? new Pagination<PurchaseDto>();
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
                return $"There was a problem when trying to load list of purchases. {ex.StatusCode}";
            }
        }

        // Return single PurchaseDto object
        public async Task<object> GetSinglePurchaseAsync(int id)
        {
            try
            {
                // Invoke API method for returning single PurchaseDto object
                var response = await client.GetAsync($"api/purchases/{id}");

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the result
                        PurchaseDto? purchaseDto = await response.Content.ReadFromJsonAsync<PurchaseDto>();

                        // If purchaseDto is not null, return purchaseDto
                        // Otherwise return new PurchaseDto object
                        return purchaseDto ?? new PurchaseDto();
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
                return $"There was a problem when trying to load requsted purchase. {ex.StatusCode}";
            }
        }

        // Return all Purchase dates
        public async Task<object> ReturnPurchaseDatesAsync()
        {
            try
            {
                // Invoke API method for returning list of Purchase dates
                var response = await client.GetAsync("api/purchases/dates");

                // If response is not null
                if (response != null)
                {
                    // If returned status code marks success
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the result
                        List<string>? purchaseDates = await response.Content.ReadFromJsonAsync<List<string>>();

                        // If purchaseDates is not null, return purchaseDates
                        // Otherwise return new List<string>
                        return purchaseDates ?? new List<string>();
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
                return $"There was a problem when trying to load requsted list of purchase dates. {ex.StatusCode}";
            }
        }
    }
}
