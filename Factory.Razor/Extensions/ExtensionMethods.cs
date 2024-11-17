using Factory.Razor.Services.Categories;
using Factory.Razor.Services.Customers;
using Factory.Razor.Services.Materials;
using Factory.Razor.Services.Products;

namespace Factory.Razor.Extensions
{
    // This static class contains extension methods for facilitating Program.cs file
    public static class ExtensionMethods
    {
        public static void AddServicesToContainer(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7080") });

            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IMaterialService, MaterialService>();
            builder.Services.AddScoped<IProductService, ProductService>();
        }
    }
}
