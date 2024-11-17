using Factory.Shared;

namespace Factory.Blazor.Services.Customers
{
    // Interface that defines methods for communicating with Server
    public interface ICustomerService
    {
        // Return paginated filtered list of CustomerDto objects
        Task<object> GetCustomersAsync(string? searchText, int pageIndex, int pageSize);
        // Return single CustomerDto object
        Task<object> GetSingleCustomerAsync(int id);
        // Create new Customer
        Task<object> CreateNewCustomerAsync(CustomerDto customerDto);
        // Edit selected Customer
        Task<object> EditCustomerAsync(CustomerDto customerDto);
        // Delete selected Customer
        Task<object> DeleteCustomerAsync(int id);
        // Return all Customers
        Task<object> GetAllCustomersAsync();
    }
}
