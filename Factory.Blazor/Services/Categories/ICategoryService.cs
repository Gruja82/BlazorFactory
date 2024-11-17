using Factory.Shared;

namespace Factory.Blazor.Services.Categories
{
    // Interface that defines methods for communicating with Server
    public interface ICategoryService
    {
        // Return paginated filtered list of CategoryDto objects
        Task<object> GetCategoriesAsync(string? searchText, int pageIndex, int pageSize);
        // Return single CategoryDto object
        Task<object> GetSingleCategoryAsync(int id);
        // Create new Category
        Task<object> CreateNewCategoryAsync(CategoryDto categoryDto);
        // Edit selected Category
        Task<object> EditCategoryAsync(CategoryDto categoryDto);
        // Delete selected Category
        Task<object> DeleteCategoryAsync(int id);
        // Return all Categories
        Task<object> GetAllCategoriesAsync();
    }
}
