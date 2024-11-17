using Factory.Shared;

namespace Factory.Blazor.Services.Orders
{
    // Interface that defines methods for communicating with Server
    public interface IOrderService
    {
        // Return paginated filtered list of OrderDto objects
        Task<object> GetOrdersAsync(string? searchText, string? stringDate, string? customer, int pageIndex, int pageSize);
        // Return single OrderDto object
        Task<object> GetSingleOrderAsync(int id);
        // Create new Order
        Task<object> CreateNewOrderAsync(OrderDto orderDto);
        // Edit selected Order
        Task<object> EditOrderAsync(OrderDto orderDto);
        // Delete selected Order
        Task<object> DeleteOrderAsync(int id);
        // Return all Order dates
        Task<object> ReturnOrderDatesAsync();
    }
}
