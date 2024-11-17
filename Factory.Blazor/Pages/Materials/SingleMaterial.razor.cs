using Factory.Blazor.Services.Categories;
using Factory.Blazor.Services.Materials;
using Factory.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Factory.Blazor.Pages.Materials
{
    // Background logic for SingleMaterial component
    public partial class SingleMaterial:IDisposable
    {
        // Inject IMaterialService
        [Inject]
        private IMaterialService MaterialService { get; set; } = default!;

        // Inject ICategoryService
        [Inject]
        private ICategoryService CategoryService { get; set; } = default!;

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

        // Route parameter
        [Parameter]
        public int Id { get; set; }

        // Property that represents form's model
        private MaterialDto? MaterialModel { get; set; }

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
                    FieldIdentifier modelField = new(MaterialModel!, error.Key);
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
            MaterialModel = new();
            Context = new(new object());
            _errors = new();
            _categories = new();
            _categories = (List<CategoryDto>)await CategoryService.GetAllCategoriesAsync();
        }

        // Method which is invoked when component parameters are set
        protected override async Task OnParametersSetAsync()
        {
            // If Id is greater than 0, then component
            // shows MaterialDto in Edit mode
            if (Id > 0)
            {
                // Set MaterialModel
                MaterialModel = (MaterialDto)await MaterialService.GetSingleMaterialAsync(Id);
                // Show Delete button
                _isHidden = false;
            }

            Context = new(MaterialModel!);
            _validationMessageStore = new(Context);
            Context.OnValidationRequested += HandleValidationRequested;
        }

        // Method which is invoked when form is submitted
        private async Task SubmitAsync()
        {
            // If Id is 0, then we have Create operation
            if (Id == 0)
            {
                // Invoke method for creating new Material
                var response = await MaterialService.CreateNewMaterialAsync(MaterialModel!);

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
                // and finally we are redirecting user to /materials page
                else
                {
                    Context!.Validate();
                    NavManager.NavigateTo("/materials");
                }
            }
            // Otherwise we have Edit operation
            else
            {
                // Invoke method for editing selected Material
                var response = await MaterialService.EditMaterialAsync(MaterialModel!);

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
                // and finally we are redirecting user to /materials page
                else
                {
                    Context!.Validate();
                    NavManager.NavigateTo("/materials");
                }
            }
        }

        // Method for handling Delete button click event
        private async Task OnDeleteClickAsync()
        {
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this Material?");

            if (confirmed)
            {
                await MaterialService.DeleteMaterialAsync(Id);
                NavManager.NavigateTo("/materials");
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
