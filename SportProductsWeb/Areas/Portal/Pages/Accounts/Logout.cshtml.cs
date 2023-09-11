using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SportProductsWeb.Areas.Portal.Pages.Accounts
{
    public class LogoutModel : PageModel
    {


        private readonly ILogger<LogoutModel> _logger;
        private readonly SignInManager<UserApplication> _signInManager;

        public LogoutModel(ILogger<LogoutModel> logger, SignInManager<UserApplication> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("./index");
        }
    }
}