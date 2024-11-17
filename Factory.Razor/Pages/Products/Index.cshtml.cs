using Factory.Razor.Services.Categories;
using Factory.Razor.Services.Products;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Factory.Razor.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;

        public IndexModel(IProductService productService, ICategoryService categoryService)
        {
            this.productService = productService;
            this.categoryService = categoryService;
            Task.FromResult(PopulateCategoriesCollectionAsync());
        }

        public Pagination<ProductDto> ProductsCollection { get; set; } = default!;
        public List<CategoryDto> CategoriesCollection { get; set; } = default!;

        public async Task OnGetAsync(string searchText, string category, int pageIndex, int pageSize)
        {
            ProductsCollection = (Pagination<ProductDto>)await productService.GetProductsAsync(searchText, category, pageIndex, pageSize);
        }

        private async Task PopulateCategoriesCollectionAsync()
        {
            CategoriesCollection = (List<CategoryDto>)await categoryService.GetAllCategoriesAsync();
        }
    }
}
