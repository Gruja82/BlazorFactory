using Factory.Razor.Services.Categories;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Factory.Razor.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ICategoryService categoryService;

        public EditModel(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
            CategoryModel = new();
        }

        [BindProperty]
        public CategoryDto CategoryModel { get; set; }

        public async Task OnGetAsync(int id)
        {
            CategoryModel = (CategoryDto)await categoryService.GetSingleCategoryAsync(id);
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (ModelState.IsValid)
            {
                var response = await categoryService.EditCategoryAsync(CategoryModel);

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

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var response = await categoryService.DeleteCategoryAsync(CategoryModel.Id);

            if (response == 204)
            {
                return RedirectToPage("/Categories/Index");
            }
            else
            {
                return Page();
            }
        }
    }
}
