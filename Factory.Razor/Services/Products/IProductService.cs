using Factory.Shared;

namespace Factory.Razor.Services.Products
{
    // Interface that defines methods for communicating with Server
    public interface IProductService
    {
        // Return paginated filtered list of ProductDto objects
        Task<object> GetProductsAsync(string? searchText, string? category, int pageIndex, int pageSize);
        // Return single ProductDto object
        Task<object> GetSingleProductAsync(int id);
        // Create new Product
        Task<object> CreateNewProductAsync(ProductDto productDto);
        // Edit selected Product
        Task<object> EditProductAsync(ProductDto productDto);
        // Delete selected Product
        Task<int> DeleteProductAsync(int id);
    }
}
