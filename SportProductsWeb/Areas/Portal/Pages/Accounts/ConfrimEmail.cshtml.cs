using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace SportProductsWeb.Areas.Portal.Pages.Accounts
{
    public class ConfrimEmailModel : PageModel
    {
        private readonly UserManager<UserApplication> userManager;

        public ConfrimEmailModel(UserManager<UserApplication> userManager)
        {
            this.userManager = userManager;
        }

        public string ResultMsg { get; set; }

        public async Task<IActionResult> OnGet(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return NotFound($"Unable to load user with Id={userId}");
            }

            code = System.Text.Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var resault = await userManager.ConfirmEmailAsync(user, code);

            if (resault.Succeeded)
            {
                ResultMsg = "Thank you for Confrimation you email";
            }
            else
            {
                ResultMsg = "Error Confriming you email";
            }

            return Page();
        }
    }
}
