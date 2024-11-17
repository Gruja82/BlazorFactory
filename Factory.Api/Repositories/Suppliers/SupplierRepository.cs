using Factory.Api.Data.Entities;
using Factory.Api.Database;
using Factory.Api.Extensions;
using Factory.Api.Utilities;
using Factory.Shared;
using Microsoft.EntityFrameworkCore;

namespace Factory.Api.Repositories.Suppliers
{
    // Implementation class for ISupplierRepository
    public class SupplierRepository:ISupplierRepository
    {
        private readonly AppDbContext context;

        public SupplierRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new Supplier
        public async Task CreateNewSupplierAsync(SupplierDto supplierDto)
        {
            // Create new Supplier object
            Supplier supplier = new();

            // Set it's property values to the ones contained in supplierDto
            supplier.Name = supplierDto.Name;
            supplier.Contact = supplierDto.Contact;
            supplier.Address = supplierDto.Address;
            supplier.City = supplierDto.City;
            supplier.Postal = supplierDto.Postal;
            supplier.Phone = supplierDto.Phone;
            supplier.Email = supplierDto.Email;

            // Add supplier to database
            await context.Suppliers.AddAsync(supplier);
        }

        // Delete selected Supplier
        public async Task DeleteSupplierAsync(int id)
        {
            // Find Supplier record from database by Primary Key value
            Supplier supplier = (await context.Suppliers.FindAsync(id))!;

            // Remove supplier from database
            context.Suppliers.Remove(supplier);
        }

        // Edit selected Supplier
        public async Task EditSupplierAsync(SupplierDto supplierDto)
        {
            // Find Supplier record from database by Primary Key value
            Supplier supplier = (await context.Suppliers.FindAsync(supplierDto.Id))!;

            // Set it's property values to the ones contained in supplierDto
            supplier.Name = supplierDto.Name;
            supplier.Contact = supplierDto.Contact;
            supplier.Address = supplierDto.Address;
            supplier.City = supplierDto.City;
            supplier.Postal = supplierDto.Postal;
            supplier.Phone = supplierDto.Phone;
            supplier.Email = supplierDto.Email;
        }

        // Return single SupplierDto object
        public async Task<SupplierDto> GetSingleSupplierAsync(int id)
        {
            // Find Supplier record from database by Primary Key value
            Supplier? supplier = await context.Suppliers.FindAsync(id);

            return supplier != null ? supplier.ConvertToDto() : new SupplierDto();
        }

        // Return paginated collection of SupplierDto objects
        public async Task<Pagination<SupplierDto>> GetSuppliersCollectionAsync(string? searchText, int pageIndex, int pageSize)
        {
            // Return all Supplier objects
            var allSuppliers = context.Suppliers
                .AsNoTracking()
                .AsQueryable();

            // If searchText is not null or empty string,
            // then filter allSuppliers by searchText
            if (!string.IsNullOrEmpty(searchText))
            {
                allSuppliers = allSuppliers.Where(e => e.Name.ToLower().Contains(searchText.ToLower())
                    || e.Contact.ToLower().Contains(searchText.ToLower())
                    || e.Email.ToLower().Contains(searchText.ToLower()))
                    .AsNoTracking()
                    .AsQueryable();
            }

            // Variable that will contain SupplierDto objects
            List<SupplierDto> supplierDtos = new();

            // Iterate through allSuppliers and populate supplierDtos
            foreach (var supplier in allSuppliers)
            {
                supplierDtos.Add(supplier.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult method, 
            // return Pagination<SupplierDto> object
            var paginatedResult = PaginationUtility<SupplierDto>.GetPaginatedResult(in supplierDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidateSupplierAsync(SupplierDto supplierDto)
        {
            // Variable that will contain possible validation errors
            Dictionary<string, string> errors = new();

            // Variable that contains all Supplier records
            var allSuppliers = context.Suppliers
                .AsNoTracking()
                .AsQueryable();

            // If supplierDto's Id value is larger than 0
            // it means that Supplier is used in Edit operation
            if (supplierDto.Id > 0)
            {
                // Find Supplier record from database by Primary Key value
                Supplier supplier = (await context.Suppliers.FindAsync(supplierDto.Id))!;

                // If supplierDto's Name value is not equal to supplier's
                // Name value, it means that user has modified Name value. 
                // Therefore we check for Name uniqueness among all Supplier records
                if (supplier.Name != supplierDto.Name)
                {
                    // If supplierDto's Name value is already contained
                    // in any of the Supplier records in database,
                    // then add validation error to errors Dictionary
                    if (allSuppliers.Select(e => e.Name.ToLower()).Contains(supplierDto.Name.ToLower()))
                    {
                        errors.Add("Name", "There is already Supplier with this Name in database. Please provide different Name.");
                    }
                }

                // If supplierDto's Email value is not equal to supplier's
                // Email value, it means that user has modified Email value. 
                // Therefore we check for Email uniqueness among all Supplier records
                if (supplier.Email != supplierDto.Email)
                {
                    // If supplierDto's Email value is already contained
                    // in any of the Supplier records in database,
                    // then add validation error to errors Dictionary
                    if (allSuppliers.Select(e => e.Email.ToLower()).Contains(supplierDto.Email.ToLower()))
                    {
                        errors.Add("Email", "There is already Supplier with this Email in database. Please provide different Email.");
                    }
                }
            }
            // Otherwise it means that Supplier is used in Create operation
            else
            {
                // If supplierDto's Name value is already contained
                // in any of the Supplier records in database,
                // then add validation error to errors Dictionary
                if (allSuppliers.Select(e => e.Name.ToLower()).Contains(supplierDto.Name.ToLower()))
                {
                    errors.Add("Name", "There is already Supplier with this Name in database. Please provide different Name.");
                }

                // If supplierDto's Email value is already contained
                // in any of the Supplier records in database,
                // then add validation error to errors Dictionary
                if (allSuppliers.Select(e => e.Email.ToLower()).Contains(supplierDto.Email.ToLower()))
                {
                    errors.Add("Email", "There is already Supplier with this Email in database. Please provide different Email.");
                }
            }

            return errors;
        }

        // Return all SupplierDto objects
        public async Task<List<SupplierDto>> GetAllSuppliersAsync()
        {
            // Return all Supplier records
            var allSuppliers = context.Suppliers
                .AsNoTracking()
                .AsQueryable();

            // Variable that will hold SupplierDto objects
            List<SupplierDto> supplierDtos = new();

            // Iterate through allSuppliers and populate supplierDtos
            // using Supplier's extension method ConvertToDto
            foreach (var supplier in allSuppliers)
            {
                supplierDtos.Add(supplier.ConvertToDto());
            }

            return await Task.FromResult(supplierDtos);
        }
    }
}
