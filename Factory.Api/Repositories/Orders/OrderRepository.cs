using Factory.Api.Data.Entities;
using Factory.Api.Database;
using Factory.Api.Extensions;
using Factory.Api.Utilities;
using Factory.Shared;
using Microsoft.EntityFrameworkCore;

namespace Factory.Api.Repositories.Orders
{
    // Implementation class for IOrderRepository
    public class OrderRepository:IOrderRepository
    {
        private readonly AppDbContext context;

        public OrderRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new Order
        public async Task CreateNewOrderAsync(OrderDto orderDto)
        {
            // Create new Order object
            Order order = new();

            // Set it's properties to the ones contained in orderDto
            order.Code = orderDto.Code;
            order.OrderDate = orderDto.OrderDate;
            order.Customer = (await context.Customers.Where(e => e.Name == orderDto.CustomerName).FirstOrDefaultAsync())!;

            // Add order to database
            await context.Orders.AddAsync(order);

            // Iterate through orderDto.OrderDetailsList,
            // set properties of each orderDetail to the ones
            // contained in orderDto.OrderDetailsList,
            // and add each orderDetail to database
            foreach (var orderDetailDto in orderDto.OrderDetailsList)
            {
                OrderDetail orderDetail = new();

                orderDetail.Order = order;
                orderDetail.Product = (await context.Products.Where(e => e.Name == orderDetailDto.ProductName).FirstOrDefaultAsync())!;
                orderDetail.Qty = orderDetailDto.Qty;
                orderDetail.Product.Quantity -= orderDetailDto.Qty;

                await context.OrderDetails.AddAsync(orderDetail);
            }
        }

        // Delete selected Order
        public async Task DeleteOrderAsync(int id)
        {
            // Find Order record from database by Primary Key value
            Order order = (await context.Orders
                .Include(e => e.OrderDetails)
                .ThenInclude(e => e.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id))!;

            // Delete all OrderDetail records
            // contained in order.OrderDetails
            foreach (var orderDetail in order.OrderDetails)
            {
                context.OrderDetails.Remove(orderDetail);

                orderDetail.Product.Quantity += orderDetail.Qty;
            }

            // Remove order from database
            context.Orders.Remove(order);
        }

        // Edit selected Order
        public async Task EditOrderAsync(OrderDto orderDto)
        {
            // Find Order record from database by Primary Key value
            Order order = (await context.Orders
                .Include(e => e.Customer)
                .Include(e => e.OrderDetails)
                .ThenInclude(e => e.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == orderDto.Id))!;

            // Set it's properties to the ones contained in orderDto
            order.Code = orderDto.Code;
            order.Customer = (await context.Customers.Where(e => e.Name == orderDto.CustomerName).FirstOrDefaultAsync())!;
            order.OrderDate = orderDto.OrderDate;

            // Delete all OrderDetail records
            // contained in order.OrderDetails
            foreach (var orderDetail in order.OrderDetails)
            {
                context.OrderDetails.Remove(orderDetail);

                orderDetail.Product.Quantity += orderDetail.Qty;
            }

            // Iterate through orderDto.OrderDetailsList,
            // set properties of each orderDetail to the ones
            // contained in orderDto.OrderDetailsList,
            // and add each orderDetail to database
            foreach (var orderDetailDto in orderDto.OrderDetailsList)
            {
                OrderDetail orderDetail = new();

                orderDetail.Order = order;
                orderDetail.Product = (await context.Products.Where(e => e.Name == orderDetailDto.ProductName).FirstOrDefaultAsync())!;
                orderDetail.Qty = orderDetailDto.Qty;
                orderDetail.Product.Quantity -= orderDetailDto.Qty;

                await context.OrderDetails.AddAsync(orderDetail);
            }
        }

        // Return paginated collection of OrderDto objects
        public async Task<Pagination<OrderDto>> GetOrdersCollectionAsync(string? searchText, string? stringDate, string? customer, int pageIndex, int pageSize)
        {
            // Return all Order objects
            var allOrders = context.Orders
                .Include(e => e.Customer)
                .Include(e => e.OrderDetails)
                .ThenInclude(e => e.Product)
                .AsNoTracking()
                .AsQueryable();

            // If searchText is not null or empty string,
            // then filter allOrders by searchText
            if (!string.IsNullOrEmpty(searchText))
            {
                allOrders = allOrders.Where(e => e.Code.ToLower().Contains(searchText.ToLower()))
                    .Include(e => e.Customer)
                    .Include(e => e.OrderDetails)
                    .ThenInclude(e => e.Product)
                    .AsNoTracking()
                    .AsQueryable();
            }

            // If orderDate is not null or empty string,
            // then filter allOrders by orderDate
            if (!string.IsNullOrEmpty(stringDate))
            {
                DateTime orderDate = DateTime.Parse(stringDate);

                allOrders = allOrders.Where(e => e.OrderDate == orderDate)
                    .Include(e => e.Customer)
                    .Include(e => e.OrderDetails)
                    .ThenInclude(e => e.Product)
                    .AsNoTracking()
                    .AsQueryable();
            }

            // If customer is not null or empty string,
            // then filter allOrders by customer
            if (!string.IsNullOrEmpty(customer))
            {
                allOrders = allOrders.Where(e => e.Customer == context.Customers.FirstOrDefault(e => e.Name == customer))
                    .Include(e => e.Customer)
                    .Include(e => e.OrderDetails)
                    .ThenInclude(e => e.Product)
                    .AsNoTracking()
                    .AsQueryable();
            }

            // Variable that will contain OrderDto objects
            List<OrderDto> orderDtos = new();

            // Iterate through allOrders and populate orderDtos
            foreach (var order in allOrders)
            {
                orderDtos.Add(order.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult method, 
            // return Pagination<OrderDto> object
            var paginatedResult = PaginationUtility<OrderDto>.GetPaginatedResult(in orderDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Return single OrderDto object
        public async Task<OrderDto> GetSingleOrderAsync(int id)
        {
            // Find Order record from database by Primary Key value
            Order? order = await context.Orders
                    .Include(e => e.Customer)
                    .Include(e => e.OrderDetails)
                    .ThenInclude(e => e.Product)
                    .FirstOrDefaultAsync(e => e.Id == id);

            return order != null ? order.ConvertToDto() : new OrderDto();
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidateOrderAsync(OrderDto orderDto)
        {
            // Variable that will contain possible validation errors
            Dictionary<string, string> errors = new();

            // Variable that contains all Order records
            var allOrders = context.Orders
                .Include(e => e.Customer)
                .Include(e => e.OrderDetails)
                .ThenInclude(e => e.Product)
                .AsNoTracking()
                .AsQueryable();

            // If orderDto's Id value is larger than 0
            // it means that Order is used in Edit operation
            if (orderDto.Id > 0)
            {
                // Find Order record from database by Primary Key value
                Order order = (await context.Orders
                    .Include(e => e.Customer)
                    .Include(e => e.OrderDetails)
                    .ThenInclude(e => e.Product)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == orderDto.Id))!;

                // If orderDtoDto's Code value is not equal to orders's
                // Code value, it means that user has modified Code value. 
                // Therefore we check for Code uniqueness among all Order records
                if (order.Code != orderDto.Code)
                {
                    // If orderDto's Code value is already contained
                    // in any of the Order records in database,
                    // then add validation error to errors Dictionary
                    if (allOrders.Select(e => e.Code.ToLower()).Contains(orderDto.Code.ToLower()))
                    {
                        errors.Add("Code", "There is already Order with this Code in database. Please provide different Code.");
                    }
                }
            }
            // Otherwise it means that Order is used in Create operation
            else
            {
                // If orderDto's Code value is already contained
                // in any of the Order records in database,
                // then add validation error to errors Dictionary
                if (allOrders.Select(e => e.Code.ToLower()).Contains(orderDto.Code.ToLower()))
                {
                    errors.Add("Code", "There is already Order with this Code in database. Please provide different Code.");
                }
            }

            // Validate that user has entered at least
            // one product in Order's products list
            if (orderDto.OrderDetailsList.Count <= 0)
            {
                errors.Add("OrderDetailsList", "There must be at least one Product in order's products list!");
            }

            foreach (var orderDetailDto in orderDto.OrderDetailsList)
            {
                Product product = (await context.Products.Where(e => e.Name == orderDetailDto.ProductName).FirstOrDefaultAsync())!;

                if (product.Quantity < orderDetailDto.Qty)
                {
                    errors.Add("OrderDetailsList", "The product's quantity you are trying to add to order is greater than product's avaliable quantity!");
                }
            }

            return errors;
        }

        // Return all Order dates
        public async Task<List<string>> GetAllOrderDatesAsync()
        {
            // Return all Orders
            var allOrders = context.Orders
                .AsNoTracking()
                .AsQueryable();

            // Variable that will hold Order dates
            List<string> orderDates = new();

            // Iterate through allOrders and populate orderDates
            foreach (var order in allOrders)
            {
                orderDates.Add(order.OrderDate.ToShortDateString());
            }

            return await Task.FromResult(orderDates.Distinct().ToList());
        }
    }
}
