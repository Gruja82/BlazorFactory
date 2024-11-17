using Factory.Shared;

namespace Factory.Blazor.Services.Suppliers
{
    // Interface that defines methods for communicating with Server
    public interface ISupplierService
    {
        // Return paginated filtered list of SupplierDto objects
        Task<object> GetSuppliersAsync(string? searchText, int pageIndex, int pageSize);
        // Return single SupplierDto object
        Task<object> GetSingleSupplierAsync(int id);
        // Create new Supplier
        Task<object> CreateNewSupplierAsync(SupplierDto supplierDto);
        // Edit selected Supplier
        Task<object> EditSupplierAsync(SupplierDto supplierDto);
        // Delete selected Supplier
        Task<object> DeleteSupplierAsync(int id);
        // Return all Suppliers
        Task<object> GetAllSuppliersAsync();
    }
}
