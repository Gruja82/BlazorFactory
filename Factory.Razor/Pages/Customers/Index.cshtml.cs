using Factory.Razor.Services.Customers;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Factory.Razor.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerService customerService;

        public IndexModel(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        public Pagination<CustomerDto> CustomersCollection { get; set; } = default!;

        public async Task OnGetAsync(string searchText, int pageIndex, int pageSize)
        {
            CustomersCollection = (Pagination<CustomerDto>)await customerService.GetCustomersAsync(searchText, pageIndex, pageSize);
        }
    }
}
