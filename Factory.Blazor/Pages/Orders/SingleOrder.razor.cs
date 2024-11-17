using Factory.Blazor.Services.Customers;
using Factory.Blazor.Services.Orders;
using Factory.Blazor.Services.Products;
using Factory.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Factory.Blazor.Pages.Orders
{
    // Background logic for SingleOrder component
    public partial class SingleOrder:IDisposable
    {
        // Inject IOrderService
        [Inject]
        private IOrderService OrderService { get; set; } = default!;

        // Inject ICustomerService
        [Inject]
        private ICustomerService CustomerService { get; set; } = default!;

        // Inject IProductService
        [Inject]
        private IProductService ProductService { get; set; } = default!;

        // Inject Navigation Manager
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        // Inject IJSRuntime
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        // Field that represents if Delete
        // button is visible or not
        private bool _isHidden = true;

        // Form's EditContext
        private EditContext? Context { get; set; }

        // Field that holds validation messages
        private ValidationMessageStore? _validationMessageStore;

        // Field that holds validation errors
        private Dictionary<string, string>? _errors;

        // Field that holds all Customer records
        private List<CustomerDto>? _customers;

        // Field that holds all Product records
        private List<ProductDto>? _products;

        // Field that holds Product name
        private string? _productName;

        // Field that holds Product quantity
        private int _productQty;

        // Field that holds error message
        private string? _error;

        // Route parameter
        [Parameter]
        public int Id { get; set; }

        // Property that represents form's model
        private OrderDto? OrderModel { get; set; }

        private void OnProductSelected(string productName)
        {
            _productName = productName;
        }

        private void OnProductQuantityChanged(int productQty)
        {
            _productQty = productQty;
        }

        private void AddOrderDetailToModel()
        {
            _error = string.Empty;

            if (!string.IsNullOrEmpty(_productName) && _productQty > 0)
            {
                if (!OrderModel!.OrderDetailsList.Select(e => e.ProductName).Contains(_productName))
                {
                    OrderDetailDto orderDetailDto = new();

                    orderDetailDto.OrderCode = OrderModel.Code;
                    orderDetailDto.ProductName = _productName;
                    orderDetailDto.Qty = _productQty;

                    OrderModel.OrderDetailsList.Add(orderDetailDto);
                }
                else
                {
                    _error = "This Product is already added to list.";
                }
            }
            else
            {
                _error = "Please check if you selected product from the list, and that quantity is greater than 0.";
            }
        }

        private void RemoveProduct(int number)
        {
            OrderModel!.OrderDetailsList.RemoveAt(number - 1);
        }

        // Method for manual activating model validation
        private void HandleValidationRequested(object? sender, ValidationRequestedEventArgs args)
        {
            // Clear _validationMessageStore
            _validationMessageStore?.Clear();

            // If there are any validation errors
            // then for each error in _errors
            // add validation message to _validationMessageStore
            if (_errors!.Any())
            {
                foreach (var error in _errors!)
                {
                    FieldIdentifier modelField = new(OrderModel!, error.Key);
                    _validationMessageStore!.Add(modelField, error.Value);
                }

                // Clear _errors field
                _errors.Clear();
            }
        }

        // Method which is invoked when component is loaded
        // and in it we initialize all fields and properties
        protected override async Task OnInitializedAsync()
        {
            OrderModel = new();
            Context = new(new object());
            _errors = new();
            _customers = new();
            _products = new();
            _productName = string.Empty;
            _error = string.Empty;
            _customers = (List<CustomerDto>)await CustomerService.GetAllCustomersAsync();
            _products = (List<ProductDto>)await ProductService.GetAllProductsAsync();
        }

        // Method which is invoked when component parameters are set
        protected override async Task OnParametersSetAsync()
        {
            // If Id is greater than 0, then component
            // shows OrderDto in Edit mode
            if (Id > 0)
            {
                // Set OrderModel
                OrderModel = (OrderDto)await OrderService.GetSingleOrderAsync(Id);
                // Show Delete button
                _isHidden = false;
            }

            Context = new(OrderModel!);
            _validationMessageStore = new(Context);
            Context.OnValidationRequested += HandleValidationRequested;
        }

        // Method which is invoked when form is submitted
        private async Task SubmitAsync()
        {
            // If Id is 0, then we have Create operation
            if (Id == 0)
            {
                // Invoke method for creating new Order
                var response = await OrderService.CreateNewOrderAsync(OrderModel!);

                // If response is of type Dictionary<string, string>,
                // then it means we have validation errors,
                // and we are converting response to Dictionary<string, string>,
                // and we are invoking method for model validation
                if (response.GetType() == typeof(Dictionary<string, string>))
                {
                    _errors = (Dictionary<string, string>)response;
                    Context!.Validate();
                }
                // Otherwise it means that Create operation was succesfull
                // and we are again invoking method for model validation
                // so that we clear previous error messages,
                // and finally we are redirecting user to /orders page
                else
                {
                    Context!.Validate();
                    NavManager.NavigateTo("/orders");
                }
            }
            // Otherwise we have Edit operation
            else
            {
                // Invoke method for editing selected Order
                var response = await OrderService.EditOrderAsync(OrderModel!);

                // If response is of type Dictionary<string, string>,
                // then it means we have validation errors,
                // and we are converting response to Dictionary<string, string>,
                // and we are invoking method for model validation
                if (response.GetType() == typeof(Dictionary<string, string>))
                {
                    _errors = (Dictionary<string, string>)response;
                    Context!.Validate();
                }
                // Otherwise it means that Create operation was succesfull
                // and we are again invoking method for model validation
                // so that we clear previous error messages,
                // and finally we are redirecting user to /orders page
                else
                {
                    Context!.Validate();
                    NavManager.NavigateTo("/orders");
                }
            }
        }

        // Method for handling Delete button click event
        private async Task OnDeleteClickAsync()
        {
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this Order?");

            if (confirmed)
            {
                await OrderService.DeleteOrderAsync(Id);
                NavManager.NavigateTo("/orders");
            }
        }

        public void Dispose()
        {
            if (Context is not null)
            {
                Context.OnValidationRequested -= HandleValidationRequested;
            }
        }
    }
}
