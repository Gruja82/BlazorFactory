using Factory.Razor.Services.Categories;
using Factory.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Factory.Razor.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ICategoryService categoryService;

        public IndexModel(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public Pagination<CategoryDto> CategoriesCollection { get; set; } = default!;

        public async Task OnGetAsync(string searchText, int pageIndex, int pageSize)
        {
            CategoriesCollection = (Pagination<CategoryDto>)await categoryService.GetCategoriesAsync(searchText, pageIndex, pageSize);
        }
    }
}
