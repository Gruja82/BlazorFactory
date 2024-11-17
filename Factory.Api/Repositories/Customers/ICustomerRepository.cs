using Factory.Shared;

namespace Factory.Api.Repositories.Customers
{
    // Interface that declares operations that 
    // can be applied on CustomerDto objects
    public interface ICustomerRepository
    {
        // Return collection of CustomerDto objects
        Task<Pagination<CustomerDto>> GetCustomersCollectionAsync(string? searchText, int pageIndex, int pageSize);
        // Return single CustomerDto object
        Task<CustomerDto> GetSingleCustomerAsync(int id);
        // Create new Customer
        Task CreateNewCustomerAsync(CustomerDto customerDto);
        // Edit selected Customer
        Task EditCustomerAsync(CustomerDto customerDto);
        // Delete selected Customer
        Task DeleteCustomerAsync(int id);
        // Custom validation
        Task<Dictionary<string, string>> ValidateCustomerAsync(CustomerDto customerDto);
        // Return all CustomerDto objects
        Task<List<CustomerDto>> GetAllCustomersAsync();
    }
}
