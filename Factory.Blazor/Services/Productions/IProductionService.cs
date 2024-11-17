using Factory.Shared;

namespace Factory.Blazor.Services.Productions
{
    // Interface that defines methods for communicating with Server
    public interface IProductionService
    {
        // Return paginated filtered list of ProductionDto objects
        Task<object> GetProductionsAsync(string? searchText, string? stringDate, string? productName, int pageIndex, int pageSize);
        // Return single ProductionDto object
        Task<object> GetSingleProductionAsync(int id);
        // Create new Production
        Task<object> CreateNewProductionAsync(ProductionDto productionDto);
        // Edit selected Production
        Task<object> EditProductionAsync(ProductionDto productionDto);
        // Delete selected Production
        Task<object> DeleteProductionAsync(int id);
        // Return all Production dates
        Task<object> ReturnProductionDatesAsync();
    }
}
