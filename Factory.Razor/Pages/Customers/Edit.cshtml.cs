using Factory.Razor.Services.Customers;
using Factory.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Factory.Razor.Pages.Customers
{
    public class EditModel : PageModel
    {
        private readonly ICustomerService customerService;

        public EditModel(ICustomerService customerService)
        {
            this.customerService = customerService;
            CustomerModel = new();
        }

        [BindProperty]
        public CustomerDto CustomerModel { get; set; }

        public async Task OnGetAsync(int id)
        {
            CustomerModel = (CustomerDto)await customerService.GetSingleCustomerAsync(id);
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (ModelState.IsValid)
            {
                var response = await customerService.EditCustomerAsync(CustomerModel);

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

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var response = await customerService.DeleteCustomerAsync(CustomerModel.Id);

            if (response == 204)
            {
                return RedirectToPage("/Customers/Index");
            }
            else
            {
                return Page();
            }
        }
    }
}
