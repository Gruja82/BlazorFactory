using Factory.Api.Database;
using Factory.Api.Repositories.Categories;
using Factory.Api.Repositories.Customers;
using Factory.Api.Repositories.Materials;
using Factory.Api.Repositories.Orders;
using Factory.Api.Repositories.Productions;
using Factory.Api.Repositories.Products;
using Factory.Api.Repositories.Purchases;
using Factory.Api.Repositories.Suppliers;
using Microsoft.EntityFrameworkCore;

namespace Factory.Api.Repositories.UoW
{
    // Implementation class for IUnitOfWork
    public class UnitOfWork:IUnitOfWork
    {
        private readonly AppDbContext context;

        public UnitOfWork(AppDbContext context, [AsParameters] UoWParameterClass parameterClass)
        {
            this.context = context;
            CategoryRepository = parameterClass.CategoryRepository;
            CustomerRepository = parameterClass.CustomerRepository;
            MaterialRepository = parameterClass.MaterialRepository;
            ProductRepository = parameterClass.ProductRepository;
            OrderRepository = parameterClass.OrderRepository;
            SupplierRepository = parameterClass.SupplierRepository;
            PurchaseRepository = parameterClass.PurchaseRepository;
            ProductionRepository = parameterClass.ProductionRepository;
        }

        public ICategoryRepository CategoryRepository { get; }
        public ICustomerRepository CustomerRepository { get; }
        public IMaterialRepository MaterialRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public ISupplierRepository SupplierRepository { get; }
        public IPurchaseRepository PurchaseRepository { get; }
        public IProductionRepository ProductionRepository { get; }

        // Save changes
        public async Task ConfirmChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        // Method for manual disposing DbContext object
        public void Dispose()
        {
            context.Dispose();
            GC.SuppressFinalize(this);
        }

        // Rollback changes
        public void RollBackChanges()
        {
            foreach (var entityEntry in context.ChangeTracker.Entries())
            {
                switch (entityEntry.State)
                {
                    case EntityState.Deleted:
                        entityEntry.Reload();
                        break;
                    case EntityState.Modified:
                        entityEntry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entityEntry.State = EntityState.Detached;
                        break;
                }
            }
        }
    }
}
