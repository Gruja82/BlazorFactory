using Factory.Shared;

namespace Factory.Api.Repositories.Suppliers
{
    // Interface that declares operations that 
    // can be applied on SupplierDto objects
    public interface ISupplierRepository
    {
        // Return collection of SupplierDto objects
        Task<Pagination<SupplierDto>> GetSuppliersCollectionAsync(string? searchText, int pageIndex, int pageSize);
        // Return single SupplierDto object
        Task<SupplierDto> GetSingleSupplierAsync(int id);
        // Create new Supplier
        Task CreateNewSupplierAsync(SupplierDto supplierDto);
        // Edit selected Supplier
        Task EditSupplierAsync(SupplierDto supplierDto);
        // Delete selected Supplier
        Task DeleteSupplierAsync(int id);
        // Custom validation
        Task<Dictionary<string, string>> ValidateSupplierAsync(SupplierDto supplierDto);
        // Return all SupplierDto objects
        Task<List<SupplierDto>> GetAllSuppliersAsync();
    }
}
