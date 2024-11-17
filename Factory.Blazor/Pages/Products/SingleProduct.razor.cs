using Factory.Blazor.Services.Categories;
using Factory.Blazor.Services.Materials;
using Factory.Blazor.Services.Products;
using Factory.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Factory.Blazor.Pages.Products
{
    // Background logic for SingleProduct component
    public partial class SingleProduct:IDisposable
    {
        // Inject IProductService
        [Inject]
        private IProductService ProductService { get; set; } = default!;

        // Inject ICategoryService
        [Inject]
        private ICategoryService CategoryService { get; set; } = default!;

        // Inject IMaterialService
        [Inject]
        private IMaterialService MaterialService { get; set; } = default!;

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

        // Field that holds all Category records
        private List<CategoryDto>? _categories;

        // Field that holds all Material records
        private List<MaterialDto>? _materials;

        // Field that holds Material name
        private string? _materialName;

        // Field that holds Material quantity
        private int _materialQty;

        // Field that holds error message
        private string? _error;

        // Route parameter
        [Parameter]
        public int Id { get; set; }

        // Property that represents form's model
        private ProductDto? ProductModel { get; set; }

        private void OnMaterialSelected(string materialName)
        {
            _materialName = materialName;
        }

        private void OnMaterialQuantityChanged(int materialQty)
        {
            _materialQty = materialQty;
        }

        private void AddProductDetailToModel()
        {
            _error = string.Empty;

            if (!string.IsNullOrEmpty(_materialName) && _materialQty > 0)
            {
                if (!ProductModel!.ProductDetailsList.Select(e => e.MaterialName).Contains(_materialName))
                {
                    ProductDetailDto productDetailDto = new();

                    productDetailDto.ProductName = ProductModel!.Name;
                    productDetailDto.MaterialName = _materialName!;
                    productDetailDto.Quantity = _materialQty;

                    ProductModel.ProductDetailsList.Add(productDetailDto);
                }
                else
                {
                    _error = "This material is already added to list.";
                }
            }
            else
            {
                _error = "Please check if you selected material from the list, and that quantity is greater than 0.";
            }
        }

        private void RemoveMaterial(int number)
        {
            ProductModel!.ProductDetailsList.RemoveAt(number - 1);
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
                    FieldIdentifier modelField = new(ProductModel!, error.Key);
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
            ProductModel = new();
            Context = new(new object());
            _errors = new();
            _categories = new();
            _materials = new();
            _materialName = string.Empty;
            _error = string.Empty;
            _categories = (List<CategoryDto>)await CategoryService.GetAllCategoriesAsync();
            _materials = (List<MaterialDto>)await MaterialService.GetAllMaterialsAsync();
        }

        // Method which is invoked when component parameters are set
        protected override async Task OnParametersSetAsync()
        {
            // If Id is greater than 0, then component
            // shows ProductDto in Edit mode
            if (Id > 0)
            {
                // Set ProductModel
                ProductModel = (ProductDto)await ProductService.GetSingleProductAsync(Id);
                // Show Delete button
                _isHidden = false;
            }

            Context = new(ProductModel!);
            _validationMessageStore = new(Context);
            Context.OnValidationRequested += HandleValidationRequested;
        }

        // Method which is invoked when form is submitted
        private async Task SubmitAsync()
        {
            // If Id is 0, then we have Create operation
            if (Id == 0)
            {
                // Invoke method for creating new Product
                var response = await ProductService.CreateNewProductAsync(ProductModel!);

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
                // and finally we are redirecting user to /products page
                else
                {
                    Context!.Validate();
                    NavManager.NavigateTo("/products");
                }
            }
            // Otherwise we have Edit operation
            else
            {
                // Invoke method for editing selected Product
                var response = await ProductService.EditProductAsync(ProductModel!);

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
                // and finally we are redirecting user to /products page
                else
                {
                    Context!.Validate();
                    NavManager.NavigateTo("/products");
                }
            }
        }

        // Method for handling Delete button click event
        private async Task OnDeleteClickAsync()
        {
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this Product?");

            if (confirmed)
            {
                await ProductService.DeleteProductAsync(Id);
                NavManager.NavigateTo("/products");
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
