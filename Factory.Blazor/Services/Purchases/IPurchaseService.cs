using Factory.Shared;

namespace Factory.Blazor.Services.Purchases
{
    // Interface that defines methods for communicating with Server
    public interface IPurchaseService
    {
        // Return paginated filtered list of PurchaseDto objects
        Task<object> GetPurchasesAsync(string? searchText, string? stringDate, string? supplier, int pageIndex, int pageSize);
        // Return single PurchaseDto object
        Task<object> GetSinglePurchaseAsync(int id);
        // Create new Purchase
        Task<object> CreateNewPurchaseAsync(PurchaseDto purchaseDto);
        // Edit selected Purchase
        Task<object> EditPurchaseAsync(PurchaseDto purchaseDto);
        // Delete selected Purchase
        Task<object> DeletePurchaseAsync(int id);
        // Return all Purchase dates
        Task<object> ReturnPurchaseDatesAsync();
    }
}
