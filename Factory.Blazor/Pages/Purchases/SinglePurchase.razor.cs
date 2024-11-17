using Factory.Blazor.Services.Materials;
using Factory.Blazor.Services.Purchases;
using Factory.Blazor.Services.Suppliers;
using Factory.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Factory.Blazor.Pages.Purchases
{
    // Background logic for SinglePurchase component
    public partial class SinglePurchase:IDisposable
    {
        // Inject IPurchaseService
        [Inject]
        private IPurchaseService PurchaseService { get; set; } = default!;

        // Inject ISupplierService
        [Inject]
        private ISupplierService SupplierService { get; set; } = default!;

        // Inject IMaterialService
        [Inject]
        private IMaterialService MaterialService { get; set; } = default!;

        // Inject Navigation Manager
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        // Inject IJSruntime
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

        // Field that holds all Supplier records
        private List<SupplierDto>? _suppliers;

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
        private PurchaseDto? PurchaseModel { get; set; }

        private void OnMaterialSelected(string materialName)
        {
            _materialName = materialName;
        }

        private void OnMaterialQuantityChanged(int materialQty)
        {
            _materialQty = materialQty;
        }

        private void AddPurchaseDetailToModel()
        {
            _error = string.Empty;

            if (!string.IsNullOrEmpty(_materialName) && _materialQty > 0)
            {
                if (!PurchaseModel!.PurchaseDetailList.Select(e => e.MaterialName).Contains(_materialName))
                {
                    PurchaseDetailDto purchaseDetailDto = new();

                    purchaseDetailDto.PurchaseCode = PurchaseModel.Code;
                    purchaseDetailDto.MaterialName = _materialName;
                    purchaseDetailDto.Qty = _materialQty;

                    PurchaseModel.PurchaseDetailList.Add(purchaseDetailDto);
                }
                else
                {
                    _error = "This Material is already added to list.";
                }
            }
            else
            {
                _error = "Please check if you selected material from the list, and that quantity is greater than 0.";
            }
        }

        private void RemoveMaterial(int number)
        {
            PurchaseModel!.PurchaseDetailList.RemoveAt(number - 1);
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
                    FieldIdentifier modelField = new(PurchaseModel!, error.Key);
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
            PurchaseModel = new();
            Context = new(new object());
            _errors = new();
            _suppliers = new();
            _materials = new();
            _materialName = string.Empty;
            _error = string.Empty;
            _suppliers = (List<SupplierDto>)await SupplierService.GetAllSuppliersAsync();
            _materials = (List<MaterialDto>)await MaterialService.GetAllMaterialsAsync();
        }

        // Method which is invoked when component parameters are set
        protected override async Task OnParametersSetAsync()
        {
            // If Id is greater than 0, then component
            // shows PurchaseDto in Edit mode
            if (Id > 0)
            {
                // Set PurchaseModel
                PurchaseModel = (PurchaseDto)await PurchaseService.GetSinglePurchaseAsync(Id);
                // Show Delete button
                _isHidden = false;
            }

            Context = new(PurchaseModel!);
            _validationMessageStore = new(Context);
            Context.OnValidationRequested += HandleValidationRequested;
        }

        // Method which is invoked when form is submitted
        private async Task SubmitAsync()
        {
            // If Id is 0, then we have Create operation
            if (Id == 0)
            {
                // Invoke method for creating new Purchase
                var response = await PurchaseService.CreateNewPurchaseAsync(PurchaseModel!);

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
                    NavManager.NavigateTo("/purchases");
                }
            }
            // Otherwise we have Edit operation
            else
            {
                // Invoke method for editing selected Purchase
                var response = await PurchaseService.EditPurchaseAsync(PurchaseModel!);

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
                    NavManager.NavigateTo("/purchases");
                }
            }
        }

        // Method for handling Delete button click event
        private async Task OnDeleteClickAsync()
        {
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this Purchase?");

            if (confirmed)
            {
                await PurchaseService.DeletePurchaseAsync(Id);
                NavManager.NavigateTo("/purchases");
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
