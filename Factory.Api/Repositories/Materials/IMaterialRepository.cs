using Factory.Shared;

namespace Factory.Api.Repositories.Materials
{
    // Interface that declares operations that 
    // can be applied on MaterialDto objects
    public interface IMaterialRepository
    {
        // Return collection of MaterialDto objects
        Task<Pagination<MaterialDto>> GetMaterialsCollectionAsync(string? searchText, string? categoryName, int pageIndex, int pageSize);
        // Return single MaterialDto object
        Task<MaterialDto> GetSingleMaterialAsync(int id);
        // Create new Material
        Task CreateNewMaterialAsync(MaterialDto materialDto);
        // Edit selected Material
        Task EditMaterialAsync(MaterialDto materialDto);
        // Delete selected Material
        Task DeleteMaterialAsync(int id);
        // Custom validation
        Task<Dictionary<string, string>> ValidateMaterialAsync(MaterialDto materialDto);
        // Return all Materials
        Task<List<MaterialDto>> GetAllMaterialsAsync();
    }
}
