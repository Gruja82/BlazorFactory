using Factory.Blazor.Services.Orders;
using Factory.Blazor.Services.Productions;
using Factory.Blazor.Services.Products;
using Factory.Shared;
using Microsoft.AspNetCore.Components;

namespace Factory.Blazor.Pages.Productions
{
    // Background logic for AllProductions component
    public partial class AllProductions
    {
        // Inject IProductionService
        [Inject]
        private IProductionService ProductionService { get; set; } = default!;

        // Inject IProductService
        [Inject]
        private IProductService ProductService { get; set; } = default!;

        // Inject Navigation Manager
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        // Property that represents collection of ProductionDto objects
        private Pagination<ProductionDto>? ProductionsCollection { get; set; }

        // Field that represents search term
        // used by SearchBarForProduction component
        private string? _searchText;

        // Field that represents stringDate
        // used by SearchBarForProduction component
        private string? _productionDate;

        // Field that represents product
        // used by SearchBarForProduction component
        private string? _product;

        // Field that represents collection
        // of dates represented as string
        // used by SearchBarForProduction component
        private List<string> _productionDates = new();

        // Field that represents collection
        // of CustomerDto objects used by 
        // SearchBarForProduction component
        private List<ProductDto> _products = new();

        // Field that represents page number
        // used by PaginationComponent
        private int _pageIndex;

        // Field that represents page size
        // used by PaginationComponent
        private int _pageSize;

        // When component is loaded for the first time,
        // fill ProductionsCollection by invoking ProductionService's
        // method GetProductionsAsync, fill _products collection
        // by invoking ProductService's method
        // GetAllProductsAsync, and fill _productionDates collection
        // by invoking ProductionService's method ReturnProductionDatesAsync
        protected override async Task OnInitializedAsync()
        {
            ProductionsCollection = (Pagination<ProductionDto>)await ProductionService.GetProductionsAsync(_searchText, _productionDate, _product, _pageIndex, _pageSize);
            _productionDates = (List<string>)await ProductionService.ReturnProductionDatesAsync();
            _products = (List<ProductDto>)await ProductService.GetAllProductsAsync();
        }

        // Method for handling button click event in Search component
        private async Task OnSearchAsync(string strValue)
        {
            // Set _searchText field value to the value of strValue
            _searchText = strValue;
            // Reset _pageIndex value
            _pageIndex = default!;
            // Fill the ProductionsCollection
            ProductionsCollection = (Pagination<ProductionDto>)await ProductionService.GetProductionsAsync(_searchText, _productionDate, _product, _pageIndex, _pageSize);
        }

        // Method for handling selection changed event in
        // Date drop down list in SearchBarForProduction component
        private async Task OnSelectedDateChangedAsync(string dateValue)
        {
            _productionDate = dateValue;
            _pageIndex = default!;
            // Fill the ProductionsCollection
            ProductionsCollection = (Pagination<ProductionDto>)await ProductionService.GetProductionsAsync(_searchText, _productionDate, _product, _pageIndex, _pageSize);
        }

        // Method for handling selection changed event in
        // Product drop down list in SearchBarForProduction component
        private async Task OnSelectedProductChangedAsync(string productValue)
        {
            _product = productValue;
            _pageIndex = default!;
            // Fill the ProductionsCollection
            ProductionsCollection = (Pagination<ProductionDto>)await ProductionService.GetProductionsAsync(_searchText, _productionDate, _product, _pageIndex, _pageSize);
        }

        // Method for handling PaginationComponent's page number
        // button click event
        private async Task OnPageChangedAsync(int pageNumber)
        {
            // Set _pageIndex field value to the value of pageNumber
            _pageIndex = pageNumber;
            // Fill the ProductionsCollection
            ProductionsCollection = (Pagination<ProductionDto>)await ProductionService.GetProductionsAsync(_searchText, _productionDate, _product, _pageIndex, _pageSize);
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
            // Fill the ProductionsCollection
            ProductionsCollection = (Pagination<ProductionDto>)await ProductionService.GetProductionsAsync(_searchText, _productionDate, _product, _pageIndex, _pageSize);
        }

        // Method for navigating to page for creating new Production
        private void NavigateToCreatePage()
        {
            NavManager.NavigateTo("/production");
        }

        // Method for navigating to page for editing selected Production
        private void NavigateToEditPage(int id)
        {
            NavManager.NavigateTo($"/production/{id}");
        }
    }
}
