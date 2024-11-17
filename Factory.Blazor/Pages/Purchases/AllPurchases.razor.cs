using Factory.Blazor.Services.Purchases;
using Factory.Blazor.Services.Suppliers;
using Factory.Shared;
using Microsoft.AspNetCore.Components;

namespace Factory.Blazor.Pages.Purchases
{
    // Background logic for AllPurchases component
    public partial class AllPurchases
    {
        // Inject IPurchaseService
        [Inject]
        private IPurchaseService PurchaseService { get; set; } = default!;

        // Inject ISupplierService
        [Inject]
        private ISupplierService SupplierService { get; set; } = default!;

        // Inject Navigation Manager
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        // Property that represents collection of PurchaseDto objects
        private Pagination<PurchaseDto>? PurchasesCollection { get; set; }

        // Field that represents search term
        // used by SearchBarWithDateAndCustomer component
        private string? _searchText;

        // Field that represents stringDate
        // used by SearchBarWithDateAndCustomer component
        private string? _purchaseDate;

        // Field that represents supplier
        // used by SearchBarWithDateAndCustomer component
        private string? _supplier;

        // Field that represents collection
        // of dates represented as string
        // used by SearchBarWithDateAndCustomer component
        private List<string> _purchaseDates = new();

        // Field that represents collection
        // of SupplierDto objects used by 
        // SearchBarWithDateAndCustomer component
        private List<SupplierDto> _suppliers = new();

        // Field that represents page number
        // used by PaginationComponent
        private int _pageIndex;

        // Field that represents page size
        // used by PaginationComponent
        private int _pageSize;

        // When component is loaded for the first time,
        // fill PurchasesCollection by invoking PurchaseService's
        // method GetPurchasesAsync, fill _suppliers collection
        // by invoking SupplierService's method
        // GetAllSuppliersAsync, and fill _purchaseDates collection
        // by invoking PurchaseService's method ReturnPurchaseDatesAsync
        protected override async Task OnInitializedAsync()
        {
            PurchasesCollection = (Pagination<PurchaseDto>)await PurchaseService.GetPurchasesAsync(_searchText, _purchaseDate, _supplier, _pageIndex, _pageSize);
            _purchaseDates = (List<string>)await PurchaseService.ReturnPurchaseDatesAsync();
            _suppliers = (List<SupplierDto>)await SupplierService.GetAllSuppliersAsync();
        }

        // Method for handling button click event in Search component
        private async Task OnSearchClickAsync(string strValue)
        {
            // Set _searchText field value to the value of strValue
            _searchText = strValue;
            // Reset _pageIndex value
            _pageIndex = default!;
            // Fill the PurchasesCollection
            PurchasesCollection = (Pagination<PurchaseDto>)await PurchaseService.GetPurchasesAsync(_searchText, _purchaseDate, _supplier, _pageIndex, _pageSize);
        }

        // Method for handling selection changed event in
        // Date drop down list in SearchBarWithDateAndCustomer component
        private async Task OnSelectedDateChangedAsync(string dateValue)
        {
            _purchaseDate = dateValue;
            _pageIndex = default!;
            // Fill the PurchasesCollection
            PurchasesCollection = (Pagination<PurchaseDto>)await PurchaseService.GetPurchasesAsync(_searchText, _purchaseDate, _supplier, _pageIndex, _pageSize);
        }

        // Method for handling selection changed event in
        // Supplier drop down list in SearchBarWithDateAndCustomer component
        private async Task OnSelectedSupplierChangedAsync(string supplierValue)
        {
            _supplier = supplierValue;
            _pageIndex = default!;
            // Fill the PurchasesCollection
            PurchasesCollection = (Pagination<PurchaseDto>)await PurchaseService.GetPurchasesAsync(_searchText, _purchaseDate, _supplier, _pageIndex, _pageSize);
        }

        // Method for handling PaginationComponent's page number
        // button click event
        private async Task OnPageChangedAsync(int pageNumber)
        {
            // Set _pageIndex field value to the value of pageNumber
            _pageIndex = pageNumber;
            // Fill the PurchasesCollection
            PurchasesCollection = (Pagination<PurchaseDto>)await PurchaseService.GetPurchasesAsync(_searchText, _purchaseDate, _supplier, _pageIndex, _pageSize);
        }

        // Method for handling PaginationComponent's page size
        // button click event
        private async Task OnPageSizeChangedAsync(int pageSize)
        {
            // Reset _pageIndex value
            _pageIndex = default!;
            // If pageSize value is larger than 0 (zero),
            // then set _pageSize value to the value of pageSize.
            // Otherwise, set _pageSize value to 4
            int pageValue = pageSize > 0 ? pageSize : 4;
            _pageSize = pageValue;
            // Fill the PurchasesCollection
            PurchasesCollection = (Pagination<PurchaseDto>)await PurchaseService.GetPurchasesAsync(_searchText, _purchaseDate, _supplier, _pageIndex, _pageSize);
        }

        // Method for navigating to page for creating new Purchase
        private void NavigateToCreatePage()
        {
            NavManager.NavigateTo("/purchase");
        }

        // Method for navigating to page for editing selected Purchase
        private void NavigateToEditPage(int id)
        {
            NavManager.NavigateTo($"/purchase/{id}");
        }
    }
}
