using Factory.Razor.Services.Categories;
using Factory.Razor.Services.Materials;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Factory.Razor.Pages.Materials
{
    public class IndexModel : PageModel
    {
        private readonly IMaterialService materialService;
        private readonly ICategoryService categoryService;

        public IndexModel(IMaterialService materialService, ICategoryService categoryService)
        {
            this.materialService = materialService;
            this.categoryService = categoryService;
            Task.FromResult(PopulateCategoriesCollectionAsync());
        }

        public Pagination<MaterialDto> MaterialsCollection { get; set; } = default!;

        public List<CategoryDto> CategoriesCollection { get; set; } = default!;

        public async Task OnGetAsync(string searchText, string category, int pageIndex, int pageSize)
        {
            MaterialsCollection = (Pagination<MaterialDto>)await materialService.GetMaterialsAsync(searchText, category, pageIndex, pageSize);
        }

        private async Task PopulateCategoriesCollectionAsync()
        {
            CategoriesCollection = (List<CategoryDto>)await categoryService.GetAllCategoriesAsync(); 
        }
    }
}
