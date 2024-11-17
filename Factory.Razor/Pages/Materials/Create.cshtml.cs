using Factory.Razor.Services.Categories;
using Factory.Razor.Services.Materials;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Factory.Razor.Pages.Materials
{
    public class CreateModel : PageModel
    {
        private readonly IMaterialService materialService;
        private readonly ICategoryService categoryService;

        public CreateModel(IMaterialService materialService, ICategoryService categoryService)
        {
            this.materialService = materialService;
            this.categoryService = categoryService;
            MaterialModel = new();
            CategoriesCollection = new();
        }

        [BindProperty]
        public MaterialDto MaterialModel { get; set; }

        public List<CategoryDto> CategoriesCollection { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await PopulateCategoriesCollectionAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var response = await materialService.CreateNewMaterialAsync(MaterialModel);

                if (response.GetType() == typeof(string))
                {
                    return RedirectToPage("/Materials/Index");
                }

                var errorResponse = (Dictionary<string, string>)response;

                foreach (var error in errorResponse)
                {
                    ModelState.AddModelError("MaterialModel." + error.Key, error.Value);
                }

                return Page();
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
