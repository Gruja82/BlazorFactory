using Factory.Razor.Services.Customers;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Factory.Razor.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly ICustomerService customerService;

        public CreateModel(ICustomerService customerService)
        {
            this.customerService = customerService;
            CustomerModel = new();
        }

        [BindProperty]
        public CustomerDto CustomerModel { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var response = await customerService.CreateNewCustomerAsync(CustomerModel);

                if (response.GetType() == typeof(string))
                {
                    return RedirectToPage("/Customers/Index");
                }

                var errorResponse = (Dictionary<string, string>)response;

                foreach (var error in errorResponse)
                {
                    ModelState.AddModelError("CustomerModel." + error.Key, error.Value);
                }

                return Page();
            }
            else
            {
                return Page();
            }
        }
    }
}
