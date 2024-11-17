using Factory.Blazor.Services.Categories;
using Factory.Blazor.Services.Customers;
using Factory.Blazor.Services.Materials;
using Factory.Blazor.Services.Orders;
using Factory.Blazor.Services.Productions;
using Factory.Blazor.Services.Products;
using Factory.Blazor.Services.Purchases;
using Factory.Blazor.Services.Suppliers;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Factory.Blazor.Extensions
{
    // This static class contains extension methods for facilitating Program.cs file
    public static class ExtensionMethods
    {
        public static void AddServicesToContainer(this WebAssemblyHostBuilder builder)
        {
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7080") });
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IMaterialService, MaterialService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IPurchaseService, PurchaseService>();
            builder.Services.AddScoped<IProductionService, ProductionService>();
        }
    }
}
