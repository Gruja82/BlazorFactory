using Factory.Api.Auth;
using Factory.Api.Data.Entities;
using Factory.Api.Database;
using Factory.Api.Repositories;
using Factory.Api.Repositories.Categories;
using Factory.Api.Repositories.Customers;
using Factory.Api.Repositories.Materials;
using Factory.Api.Repositories.Orders;
using Factory.Api.Repositories.Productions;
using Factory.Api.Repositories.Products;
using Factory.Api.Repositories.Purchases;
using Factory.Api.Repositories.Suppliers;
using Factory.Api.Repositories.UoW;
using Factory.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Factory.Api.Extensions
{
    // This static class contains extension methods for facilitating Program.cs
    // file and to avoid writing similar code over and over
    public static class ExtensionMethods
    {
        // This extension metod extends WebApplicationBuilder by adding functionallity
        // for registering AppDbContext and all services to DI container
        public static void AddServicesToContainer(this WebApplicationBuilder builder, string connString)
        {
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connString));
            //builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlite(connString));

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            builder.Services.AddScoped<IProductionRepository, ProductionRepository>();

            builder.Services.AddScoped(typeof(UoWParameterClass));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        // This extension method converts Category object to CategoryDto object
        public static CategoryDto ConvertToDto(this Category category)
        {
            // Create new CategoryDto object
            CategoryDto categoryDto = new();

            // Set it's property values to the ones contained in category
            categoryDto.Id = category.Id;
            categoryDto.Name = category.Name;
            categoryDto.Description = category.Description;

            return categoryDto;
        }

        // This overloaded extension method converts Customer object to CustomerDto object
        public static CustomerDto ConvertToDto(this Customer customer)
        {
            // Create new CustomerDto object
            CustomerDto customerDto = new();

            // Set it's properties values to the ones contained in customer
            customerDto.Id = customer.Id;
            customerDto.Name = customer.Name;
            customerDto.Contact = customer.Contact;
            customerDto.Address = customer.Address;
            customerDto.City = customer.City;
            customerDto.Postal = customer.Postal;
            customerDto.Phone = customer.Phone;
            customerDto.Email = customer.Email;

            return customerDto;
        }

        // This overloaded extension method converts Material object to MaterialDto object
        public static MaterialDto ConvertToDto(this Material material)
        {
            // Create new MaterialDto object
            MaterialDto materialDto = new();

            // Set it's properties to the ones contained in material
            materialDto.Id = material.Id;
            materialDto.Name = material.Name;
            materialDto.CategoryName = material.Category.Name;
            materialDto.Quantity = material.Quantity;
            materialDto.Price = material.Price;

            return materialDto;
        }

        // This overloaded extension method converts Product object to ProductDto object
        public static ProductDto ConvertToDto(this Product product)
        {
            // Create new ProductDto object
            ProductDto productDto = new();

            // Set it's properties to the ones contained in product
            productDto.Id = product.Id;
            productDto.Name = product.Name;
            productDto.CategoryName = product.Category.Name;
            productDto.Quantity = product.Quantity;
            productDto.Price = product.Price;
            foreach (var productDetail in product.ProductDetails)
            {
                productDto.ProductDetailsList.Add(productDetail.ConvertToDto());
            }

            return productDto;
        }

        // This overloaded extension method converts ProductDetail object to ProductDetailDto object
        public static ProductDetailDto ConvertToDto(this ProductDetail productDetail)
        {
            // Create new ProductDetailDto object
            ProductDetailDto productDetailDto = new();

            // Set it's properties to the ones contained in productDetail
            productDetailDto.Id = productDetail.Id;
            productDetailDto.ProductName = productDetail.Product.Name;
            productDetailDto.MaterialName = productDetail.Material.Name;
            productDetailDto.Quantity = productDetail.QtyMaterial;

            return productDetailDto;
        }

        // This overloaded extension method converts Order object to OrderDto object
        public static OrderDto ConvertToDto(this Order order)
        {
            // Create new OrderDto object
            OrderDto orderDto = new();

            // Set it's properties to the ones contained in order
            orderDto.Id = order.Id;
            orderDto.Code = order.Code;
            orderDto.OrderDate = order.OrderDate;
            orderDto.CustomerName = order.Customer.Name;

            return orderDto;
        }

        // This overloaded extension method converts OrderDetail object to OrderDetailDto object
        public static OrderDetailDto ConvertToDto(this OrderDetail orderDetail)
        {
            // Create new OrderDetailDto object
            OrderDetailDto orderDetailDto = new();

            // Set it's properties to the ones contained in orderDetail
            orderDetailDto.Id = orderDetail.Id;
            orderDetailDto.OrderCode = orderDetail.Order.Code;
            orderDetailDto.ProductName = orderDetail.Product.Name;
            orderDetailDto.Qty = orderDetail.Qty;

            return orderDetailDto;
        }

        // This overloaded extension method converts Supplier object to SupplierDto object
        public static SupplierDto ConvertToDto(this Supplier supplier)
        {
            // Create new SupplierDto object
            SupplierDto supplierDto = new();

            // Set it's properties values to the ones contained in supplier
            supplierDto.Id = supplier.Id;
            supplierDto.Name = supplier.Name;
            supplierDto.Contact = supplier.Contact;
            supplierDto.Address = supplier.Address;
            supplierDto.City = supplier.City;
            supplierDto.Postal = supplier.Postal;
            supplierDto.Phone = supplier.Phone;
            supplierDto.Email = supplier.Email;

            return supplierDto;
        }

        // This overloaded extension method converts PurchaseDetail object to PurchaseDetailDto object
        public static PurchaseDetailDto ConvertToDto(this PurchaseDetail purchaseDetail)
        {
            // Create new PurchaseDetailDto object
            PurchaseDetailDto purchaseDetailDto = new();

            // Set it's properties to the ones contained in opurchaseDetail
            purchaseDetailDto.Id = purchaseDetail.Id;
            purchaseDetailDto.PurchaseCode = purchaseDetail.Purchase.Code;
            purchaseDetailDto.MaterialName = purchaseDetail.Material.Name;
            purchaseDetailDto.Qty = purchaseDetail.Qty;

            return purchaseDetailDto;
        }

        // This overloaded extension method converts Purchase object to PurchaseDto object
        public static PurchaseDto ConvertToDto(this Purchase purchase)
        {
            // Create new PurchaseDto object
            PurchaseDto purchaseDto = new();

            // Set it's properties to the ones contained in purchase
            purchaseDto.Id = purchase.Id;
            purchaseDto.Code = purchase.Code;
            purchaseDto.PurchaseDate = purchase.PurchaseDate;
            purchaseDto.SupplierName = purchase.Supplier.Name;

            return purchaseDto;
        }

        // This overloaded extension method converts Production object to ProductionDto object
        public static ProductionDto ConvertToDto(this Production production)
        {
            // Create new ProductionDto object
            ProductionDto productionDto = new();

            // Set it's properties to the ones contained in production
            productionDto.Id = production.Id;
            productionDto.Code = production.Code;
            productionDto.ProductionDate = production.ProductionDate;
            productionDto.ProductName = production.Product.Name;
            productionDto.Qty = production.Qty;

            return productionDto;
        }
    }
}
