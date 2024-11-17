using Factory.Shared;

namespace Factory.Api.Repositories.Products
{
    // Interface that declares operations that 
    // can be applied on ProductDto objects
    public interface IProductRepository
    {
        // Return collection of ProductDto objects
        Task<Pagination<ProductDto>> GetProductsCollectionAsync(string? searchText, string? categoryName, int pageIndex, int pageSize);
        // Return single ProductDto object
        Task<ProductDto> GetSingleProductAsync(int id);
        // Create new Product
        Task CreateNewProductAsync(ProductDto productDto);
        // Edit selected Product
        Task EditProductAsync(ProductDto productDto);
        // Delete selected Product
        Task DeleteProductAsync(int id);
        // Custom validation
        Task<Dictionary<string, string>> ValidateProductAsync(ProductDto productDto);
        // Return all Products
        Task<List<ProductDto>> GetAllProductsAsync();
    }
}
