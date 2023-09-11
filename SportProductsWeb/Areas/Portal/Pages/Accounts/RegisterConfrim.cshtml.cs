using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SportProductsWeb.Areas.Portal.Pages.Accounts
{
    public class RegisterConfrimModel : PageModel
    {


        public string Email { get; set; }

        public void OnGet(string email)
        {
            if (email != null)
            {
                Email = email;
            }
        }
    }
}
