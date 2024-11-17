using Factory.Blazor.Services.Customers;
using Factory.Shared;
using Microsoft.AspNetCore.Components;

namespace Factory.Blazor.Pages.Customers
{
    // Background logic for AllCustomers component
    public partial class AllCustomers
    {
        // Inject ICustomerService
        [Inject]
        private ICustomerService CustomerService { get; set; } = default!;

        // Inject Navigation Manager
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        // Property that represents collection of CustomerDto objects
        private Pagination<CustomerDto>? CustomersCollection { get; set; }

        // Field that represents search term
        // used by Search component
        private string? _searchText;

        // Field that represents page number
        // used by PaginationComponent
        private int _pageIndex;

        // Field that represents page size
        // used by PaginationComponent
        private int _pageSize;

        // When component is loaded for the first time,
        // fill CustomersCollection by invoking CustomerService's
        // method GetCustomersAsync
        protected override async Task OnInitializedAsync()
        {
            CustomersCollection = (Pagination<CustomerDto>)await CustomerService.GetCustomersAsync(_searchText, _pageIndex, _pageSize);
        }

        // Method for handling button click event in Search component
        private async Task OnSearchAsync(string strValue)
        {
            // Set _searchText field value to the value of strValue
            _searchText = strValue;
            // Reset _pageIndex value
            _pageIndex = default!;
            // Fill the CustomersCollection
            CustomersCollection = (Pagination<CustomerDto>)await CustomerService.GetCustomersAsync(_searchText, _pageIndex, _pageSize);
        }

        // Method for handling PaginationComponent's page number
        // button click event
        private async Task OnPageChangedAsync(int pageNumber)
        {
            // Set _pageIndex field value to the value of pageNumber
            _pageIndex = pageNumber;
            // Fill the CustomersCollection
            CustomersCollection = (Pagination<CustomerDto>)await CustomerService.GetCustomersAsync(_searchText, _pageIndex, _pageSize);
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
            // Fill the CustomersCollection
            CustomersCollection = (Pagination<CustomerDto>)await CustomerService.GetCustomersAsync(_searchText, _pageIndex, _pageSize);
        }

        // Method for navigating to page for creating new Customer
        private void NavigateToCreatePage()
        {
            NavManager.NavigateTo("/customer");
        }

        // Method for navigating to page for editing selected Customer
        private void NavigateToEditPage(int id)
        {
            NavManager.NavigateTo($"/customer/{id}");
        }
    }
}
