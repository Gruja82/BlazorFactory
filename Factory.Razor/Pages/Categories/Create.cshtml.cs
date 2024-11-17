using Factory.Razor.Services.Categories;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Factory.Razor.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ICategoryService categoryService;

        public CreateModel(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
            CategoryModel = new();
        }

        [BindProperty]
        public CategoryDto CategoryModel { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var response = await categoryService.CreateNewCategoryAsync(CategoryModel);

                if (response.GetType() == typeof(string))
                {
                    return RedirectToPage("/Categories/Index");
                }

                var errorResponse = (Dictionary<string, string>)response;

                foreach (var error in errorResponse)
                {
                    ModelState.AddModelError("CategoryModel." + error.Key, error.Value);
                }

                return Page();
            }
            else
            {
                return Page();
            }
        }
    }
}
