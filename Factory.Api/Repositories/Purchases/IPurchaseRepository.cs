using Factory.Shared;

namespace Factory.Api.Repositories.Purchases
{
    // Interface that declares operations that 
    // can be applied on PurchaseDto objects
    public interface IPurchaseRepository
    {
        // Return collection of PurchaseDto objects
        Task<Pagination<PurchaseDto>> GetPurchasesCollectionAsync(string? searchText, string? stringDate, string? supplier, int pageIndex, int pageSize);
        // Return single PurchaseDto object
        Task<PurchaseDto> GetSinglePurchaseAsync(int id);
        // Create new Purchase
        Task CreateNewPurchaseAsync(PurchaseDto purchaseDto);
        // Edit selected Purchase
        Task EditPurchaseAsync(PurchaseDto purchaseDto);
        // Delete selected Purchase
        Task DeletePurchaseAsync(int id);
        // Custom validation
        Task<Dictionary<string, string>> ValidatePurchaseAsync(PurchaseDto purchaseDto);
        // Return all Purchase dates
        Task<List<string>> GetAllPurchaseDatesAsync();
    }
}
