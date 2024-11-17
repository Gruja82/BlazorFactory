using Factory.Shared;

namespace Factory.Razor.Services.Materials
{
    // Interface that defines methods for communicating with Server
    public interface IMaterialService
    {
        // Return paginated filtered list of MaterialDto objects
        Task<object> GetMaterialsAsync(string? searchText, string? category, int pageIndex, int pageSize);
        // Return single MaterialDto object
        Task<object> GetSingleMaterialAsync(int id);
        // Create new Material
        Task<object> CreateNewMaterialAsync(MaterialDto materialDto);
        // Edit selected Material
        Task<object> EditMaterialAsync(MaterialDto materialDto);
        // Delete selected Material
        Task<int> DeleteMaterialAsync(int id);
        // Return all Materials
        Task<object> GetAllMaterialsAsync();
    }
}
