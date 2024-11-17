using Factory.Razor.Services.Categories;
using Factory.Razor.Services.Materials;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Factory.Razor.Pages.Materials
{
    public class EditModel : PageModel
    {
        private readonly IMaterialService materialService;
        private readonly ICategoryService categoryService;

        public EditModel(IMaterialService materialService, ICategoryService categoryService)
        {
            this.materialService = materialService;
            this.categoryService = categoryService;
            MaterialModel = new();
            CategoriesCollection = new();
        }

        [BindProperty]
        public MaterialDto MaterialModel { get; set; }

        public List<CategoryDto> CategoriesCollection { get; set; }

        public async Task OnGetAsync(int id)
        {
            await PopulateCategoriesCollectionAsync();
            MaterialModel = (MaterialDto)await materialService.GetSingleMaterialAsync(id);
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (ModelState.IsValid)
            {
                var response = await materialService.EditMaterialAsync(MaterialModel);

                if (response.GetType() == typeof(string))
                {
                    return RedirectToPage("/Materials/Index");
                }

                var errorResponse = (Dictionary<string, string>)response;

                foreach (var error in errorResponse)
                {
                    ModelState.AddModelError("MaterialModel." + error.Key, error.Value);
                }

                await PopulateCategoriesCollectionAsync();

                return Page();
            }
            else
            {
                await PopulateCategoriesCollectionAsync();

                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var response = await materialService.DeleteMaterialAsync(MaterialModel.Id);

            if (response == 204)
            {
                return RedirectToPage("/Materials/Index");
            }
            else
            {
                return Page();
            }
        }

        private async Task PopulateCategoriesCollectionAsync()
        {
            CategoriesCollection = (List<CategoryDto>)await categoryService.GetAllCategoriesAsync();
        }
    }
}
