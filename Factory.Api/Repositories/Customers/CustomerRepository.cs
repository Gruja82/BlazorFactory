using Factory.Api.Data.Entities;
using Factory.Api.Database;
using Factory.Api.Extensions;
using Factory.Api.Utilities;
using Factory.Shared;
using Microsoft.EntityFrameworkCore;

namespace Factory.Api.Repositories.Customers
{
    // Implementation class for ICustomerRepository
    public class CustomerRepository:ICustomerRepository
    {
        private readonly AppDbContext context;

        public CustomerRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new Customer
        public async Task CreateNewCustomerAsync(CustomerDto customerDto)
        {
            // Create new Customer object
            Customer customer = new();

            // Set it's property values to the ones contained in customerDto
            customer.Name = customerDto.Name;
            customer.Contact = customerDto.Contact;
            customer.Address = customerDto.Address;
            customer.City = customerDto.City;
            customer.Postal = customerDto.Postal;
            customer.Phone = customerDto.Phone;
            customer.Email = customerDto.Email;

            // Add customer to database
            await context.Customers.AddAsync(customer);
        }

        // Delete selected Customer
        public async Task DeleteCustomerAsync(int id)
        {
            // Find Customer record from database by Primary Key value
            Customer customer = (await context.Customers.FindAsync(id))!;

            // Remove customer from database
            context.Customers.Remove(customer);
        }

        // Edit selected Customer
        public async Task EditCustomerAsync(CustomerDto customerDto)
        {
            // Find Customer record from database by Primary Key value
            Customer customer = (await context.Customers.FindAsync(customerDto.Id))!;

            // Set it's property values to the ones contained in customerDto
            customer.Name = customerDto.Name;
            customer.Contact = customerDto.Contact;
            customer.Address = customerDto.Address;
            customer.City = customerDto.City;
            customer.Postal = customerDto.Postal;
            customer.Phone = customerDto.Phone;
            customer.Email = customerDto.Email;
        }

        // Return paginated collection of CustomerDto objects
        public async Task<Pagination<CustomerDto>> GetCustomersCollectionAsync(string? searchText, int pageIndex, int pageSize)
        {
            // Return all Customer objects
            var allCustomers = context.Customers
                .AsNoTracking()
                .AsQueryable();

            // If searchText is not null or empty string,
            // then filter allCustomers by searchText
            if (!string.IsNullOrEmpty(searchText))
            {
                allCustomers = allCustomers.Where(e => e.Name.ToLower().Contains(searchText.ToLower())
                                                || e.Contact.ToLower().Contains(searchText.ToLower())
                                                || e.Email.ToLower().Contains(searchText.ToLower()))
                                                .AsNoTracking()
                                                .AsQueryable();
            }

            // Variable that will contain CustomerDto objects
            List<CustomerDto> customerDtos = new();

            // Iterate through allCustomers and populate customerDtos
            foreach (var customer in allCustomers)
            {
                customerDtos.Add(customer.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult method, 
            // return Pagination<CustomerDto> object
            var paginatedResult = PaginationUtility<CustomerDto>.GetPaginatedResult(in customerDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Return single CustomerDto object
        public async Task<CustomerDto> GetSingleCustomerAsync(int id)
        {
            // Find Customer record from database by Primary Key value
            Customer? customer = await context.Customers.FindAsync(id);

            return customer != null ? customer.ConvertToDto() : new CustomerDto();
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidateCustomerAsync(CustomerDto customerDto)
        {
            // Variable that will contain possible validation errors
            Dictionary<string, string> errors = new();

            // Variable that contains all Customer records
            var allCustomers = context.Customers
                .AsNoTracking()
                .AsQueryable();

            // If customerDto's Id value is larger than 0
            // it means that Customer is used in Edit operation
            if (customerDto.Id > 0)
            {
                // Find Customer record from database by Primary Key value
                Customer customer = (await context.Customers.FindAsync(customerDto.Id))!;

                // If customerDto's Name value is not equal to customer's
                // Name value, it means that user has modified Name value. 
                // Therefore we check for Name uniqueness among all Customer records
                if (customer.Name != customerDto.Name)
                {
                    // If customerDto's Name value is already contained
                    // in any of the Customer records in database,
                    // then add validation error to errors Dictionary
                    if (allCustomers.Select(e => e.Name.ToLower()).Contains(customerDto.Name.ToLower()))
                    {
                        errors.Add("Name", "There is already Customer with this Name in database. Please provide different Name.");
                    }
                }

                // If customerDto's Email value is not equal to customer's
                // Email value, it means that user has modified Email value.
                // Therefore we check for Email uniqueness among all Customer records
                if (customer.Email != customerDto.Email)
                {
                    // If customerDto's Email value is already contained
                    // in any of the Customer records in database,
                    // then add validation error to errors Dictionary
                    if (allCustomers.Select(e => e.Email.ToLower()).Contains(customerDto.Email.ToLower()))
                    {
                        errors.Add("Email", "There is already Customer with this Email in database. Please provide different Email.");
                    }
                }
            }
            // Otherwise it means that Customer is used in Create operation
            else
            {
                // If customerDto's Name value is already contained
                // in any of the Customer records in database,
                // then add validation error to errors Dictionary
                if (allCustomers.Select(e => e.Name.ToLower()).Contains(customerDto.Name.ToLower()))
                {
                    errors.Add("Name", "There is already Customer with this Name in database. Please provide different Name.");
                }

                // If customerDto's Email value is already contained
                // in any of the Customer records in database,
                // then add validation error to errors Dictionary
                if (allCustomers.Select(e => e.Email.ToLower()).Contains(customerDto.Email.ToLower()))
                {
                    errors.Add("Email", "There is already Customer with this Email in database. Please provide different Email.");
                }
            }

            return errors;
        }

        // Return all CustomerDto objects
        public async Task<List<CustomerDto>> GetAllCustomersAsync()
        {
            // Return all Customer records
            var allCustomers = context.Customers
                .AsNoTracking()
                .AsQueryable();

            // Variable that will hold CustomerDto objects
            List<CustomerDto> customerDtos = new();

            // Iterate through allCustomers and populate customerDtos
            // using Customer's extension method ConvertToDto
            foreach (var customer in allCustomers)
            {
                customerDtos.Add(customer.ConvertToDto());
            }

            return await Task.FromResult(customerDtos);
        }
    }
}
