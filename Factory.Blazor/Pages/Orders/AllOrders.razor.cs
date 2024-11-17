using Factory.Blazor.Services.Customers;
using Factory.Blazor.Services.Orders;
using Factory.Shared;
using Microsoft.AspNetCore.Components;

namespace Factory.Blazor.Pages.Orders
{
    // Background logic for AllOrders component
    public partial class AllOrders
    {
        // Inject IOrderService
        [Inject]
        private IOrderService OrderService { get; set; } = default!;

        // Inject ICustomerService
        [Inject]
        private ICustomerService CustomerService { get; set; } = default!;

        // Inject Navigation Manager
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        // Property that represents collection of OrderDto objects
        private Pagination<OrderDto>? OrdersCollection { get; set; }

        // Field that represents search term
        // used by SearchBarWithDateAndCustomer component
        private string? _searchText;

        // Field that represents stringDate
        // used by SearchBarWithDateAndCustomer component
        private string? _orderDate;

        // Field that represents customer
        // used by SearchBarWithDateAndCustomer component
        private string? _customer;

        // Field that represents collection
        // of dates represented as string
        // used by SearchBarWithDateAndCustomer component
        private List<string> _orderDates = new();

        // Field that represents collection
        // of CustomerDto objects used by 
        // SearchBarWithDateAndCustomer component
        private List<CustomerDto> _customers = new();

        // Field that represents page number
        // used by PaginationComponent
        private int _pageIndex;

        // Field that represents page size
        // used by PaginationComponent
        private int _pageSize;

        // When component is loaded for the first time,
        // fill OrdersCollection by invoking OrderService's
        // method GetOrdersAsync, fill _customers collection
        // by invoking CustomeryService's method
        // GetAllCustomersAsync, and fill _orderDates collection
        // by invoking OrderService's method ReturnOrderDatesAsync
        protected override async Task OnInitializedAsync()
        {
            OrdersCollection = (Pagination<OrderDto>)await OrderService.GetOrdersAsync(_searchText, _orderDate, _customer, _pageIndex, _pageSize);
            _orderDates = (List<string>)await OrderService.ReturnOrderDatesAsync();
            _customers = (List<CustomerDto>)await CustomerService.GetAllCustomersAsync();
        }

        // Method for handling button click event in Search component
        private async Task OnSearchAsync(string strValue)
        {
            // Set _searchText field value to the value of strValue
            _searchText = strValue;
            // Reset _pageIndex value
            _pageIndex = default!;
            // Fill the OrdersCollection
            OrdersCollection = (Pagination<OrderDto>)await OrderService.GetOrdersAsync(_searchText, _orderDate, _customer, _pageIndex, _pageSize);
        }

        // Method for handling selection changed event in
        // Date drop down list in SearchBarWithDateAndCustomer component
        private async Task OnSelectedDateChangedAsync(string dateValue)
        {
            _orderDate = dateValue;
            _pageIndex = default!;
            // Fill the OrdersCollection
            OrdersCollection = (Pagination<OrderDto>)await OrderService.GetOrdersAsync(_searchText, _orderDate, _customer, _pageIndex, _pageSize);
        }

        // Method for handling selection changed event in
        // Customer drop down list in SearchBarWithDateAndCustomer component
        private async Task OnSelectedCustomerChangedAsync(string customerValue)
        {
            _customer = customerValue;
            _pageIndex = default!;
            // Fill the OrdersCollection
            OrdersCollection = (Pagination<OrderDto>)await OrderService.GetOrdersAsync(_searchText, _orderDate, _customer, _pageIndex, _pageSize);
        }

        // Method for handling PaginationComponent's page number
        // button click event
        private async Task OnPageChangedAsync(int pageNumber)
        {
            // Set _pageIndex field value to the value of pageNumber
            _pageIndex = pageNumber;
            // Fill the OrdersCollection
            OrdersCollection = (Pagination<OrderDto>)await OrderService.GetOrdersAsync(_searchText, _orderDate, _customer, _pageIndex, _pageSize);
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
            // Fill the OrdersCollection
            OrdersCollection = (Pagination<OrderDto>)await OrderService.GetOrdersAsync(_searchText, _orderDate, _customer, _pageIndex, _pageSize);
        }

        // Method for navigating to page for creating new Order
        private void NavigateToCreatePage()
        {
            NavManager.NavigateTo("/order");
        }

        // Method for navigating to page for editing selected Order
        private void NavigateToEditPage(int id)
        {
            NavManager.NavigateTo($"/order/{id}");
        }
    }
}
