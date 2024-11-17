using Factory.Razor.Pages.Shared;
using Factory.Razor.Services.Categories;
using Factory.Razor.Services.Materials;
using Factory.Razor.Services.Products;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Factory.Razor.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly IProductService productService;
        private readonly IMaterialService materialService;
        private readonly ICategoryService categoryService;

        public CreateModel(IProductService productService, IMaterialService materialService, ICategoryService categoryService)
        {
            this.productService = productService;
            this.materialService = materialService;
            this.categoryService = categoryService;
            ProductDetailList = new();
            MaterialNames = new();
            CategoriesCollection = new();
            ProductModel = new();
        }
        public string MaterialName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public List<ProductDetailDto> ProductDetailList { get; set; }
        public List<string> MaterialNames { get; set; }
        public List<CategoryDto> CategoriesCollection { get; set; }

        [BindProperty]
        public ProductDto ProductModel { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await PopulateCategoriesCollectionAsync();
            await PopulateMaterialsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var response = await productService.CreateNewProductAsync(ProductModel);

                if (response.GetType() == typeof(string))
                {
                    return RedirectToPage("/Products/Index");
                }

                var errorResponse = (Dictionary<string, string>)response;

                foreach (var error in errorResponse)
                {
                    ModelState.AddModelError("ProductModel." + error.Key, error.Value);
                }

                return Page();
            }
            else
            {
                return Page();
            }
        }

        public IActionResult OnPostAddRow()
        {
            ProductDetailDto productDetailDto = new();
            productDetailDto.ProductName = ProductModel.Name;
            productDetailDto.MaterialName = MaterialName;
            productDetailDto.Quantity = Quantity;

            ProductDetailList.Add(productDetailDto);

            return RedirectToPage();
        }

        private async Task PopulateCategoriesCollectionAsync()
        {
            CategoriesCollection = (List<CategoryDto>)await categoryService.GetAllCategoriesAsync();
        }

        private async Task PopulateMaterialsAsync()
        {
            var materialList = (List<MaterialDto>)await materialService.GetAllMaterialsAsync();

            foreach (var material in materialList)
            {
                MaterialNames.Add(material.Name);
            }
        }
    }
}
