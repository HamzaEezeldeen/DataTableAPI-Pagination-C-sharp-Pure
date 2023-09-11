using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportProductsWeb.Services;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace SportProductsWeb.Areas.Portal.Pages.Accounts
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<UserApplication> _signInManager;
        private readonly UserManager<UserApplication> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        private readonly AppEmailService appEmailService;

        public LoginModel(SignInManager<UserApplication> signInManager, UserManager<UserApplication> userManager, ILogger<RegisterModel> logger, AppEmailService appEmailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            this.appEmailService = appEmailService;
        }


        public class LoginVm
        {
            [EmailAddress, Required]
            public string Email { get; set; }

            [Required, Display(Name = "Password"), DataType(DataType.Password), StringLength(100, ErrorMessage = "Enter at least {2} characters", MinimumLength = 6)]
            public string Password { get; set; }
            public bool RemmberMe { get; set; }
        }

        [BindProperty]
        public LoginVm loginvm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string retUrl { get; set; }

        public async Task OnGet()
        {
            await appEmailService.SendEmailAsync("h.k409383429@gmail.com", "hello", "<p>hello</p>");

            ViewData["Title"] = "Login";
        }

        public async Task<IActionResult> OnPost()
        {
            if (retUrl is null)
            {
                Url.Content("~/");
            }


            if (ModelState.IsValid)
            {
                var resault = await _signInManager.PasswordSignInAsync(new MailAddress(loginvm.Email).User, loginvm.Password, loginvm.RemmberMe, false);

                if (resault.Succeeded)
                {
                    _logger.LogInformation($"User {loginvm.Email} logged in");

                    return RedirectToPage(retUrl);
                }

                else if (resault.IsLockedOut)
                {
                    _logger.LogInformation($"User {loginvm.Email} is locked out");

                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to login!!");
                }
            }
            return Page();
        }


    }
}