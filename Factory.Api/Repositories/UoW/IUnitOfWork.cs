using Factory.Api.Repositories.Categories;
using Factory.Api.Repositories.Customers;
using Factory.Api.Repositories.Materials;
using Factory.Api.Repositories.Orders;
using Factory.Api.Repositories.Productions;
using Factory.Api.Repositories.Products;
using Factory.Api.Repositories.Purchases;
using Factory.Api.Repositories.Suppliers;

namespace Factory.Api.Repositories.UoW
{
    // Interface that wraps up specific entity repositories
    // and declares methods for saving changes to database,
    // and rolling back changes made to database
    public interface IUnitOfWork:IDisposable
    {
        public ICategoryRepository CategoryRepository { get; }
        public ICustomerRepository CustomerRepository { get; }
        public IMaterialRepository MaterialRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public ISupplierRepository SupplierRepository { get; }
        public IPurchaseRepository PurchaseRepository { get; }
        public IProductionRepository ProductionRepository { get; }

        // Save changes
        Task ConfirmChangesAsync();
        // Rollback changes
        void RollBackChanges();
    }
}
