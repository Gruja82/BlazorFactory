using Factory.Api.Repositories.Categories;
using Factory.Api.Repositories.Customers;
using Factory.Api.Repositories.Materials;
using Factory.Api.Repositories.Orders;
using Factory.Api.Repositories.Productions;
using Factory.Api.Repositories.Products;
using Factory.Api.Repositories.Purchases;
using Factory.Api.Repositories.Suppliers;

namespace Factory.Api.Repositories
{
    public interface IUoWParameterClass
    {
        public ICategoryRepository CategoryRepository { get; set; }
        public ICustomerRepository CustomerRepository { get; set; }
        public IMaterialRepository MaterialRepository { get; set; }
        public IOrderRepository OrderRepository { get; set; }
        public IProductRepository ProductRepository { get; set; }
        public ISupplierRepository SupplierRepository { get; set; }
        public IPurchaseRepository PurchaseRepository { get; set; }
        public IProductionRepository ProductionRepository { get; set; }
    }
}
