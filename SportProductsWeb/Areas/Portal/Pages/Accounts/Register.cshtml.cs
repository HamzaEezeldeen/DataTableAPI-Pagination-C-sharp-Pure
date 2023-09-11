using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SportProductsWeb.Services;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;

namespace SportProductsWeb.Areas.Portal.Pages.Accounts
{
    public class RegisterModel : PageModel
    {

        private readonly SignInManager<UserApplication> _signInManager;
        private readonly UserManager<UserApplication> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly AppEmailService appEmailService;

        public RegisterModel(SignInManager<UserApplication> signInManager, UserManager<UserApplication> userManager, ILogger<RegisterModel> logger, AppEmailService appEmailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            this.appEmailService = appEmailService;
        }

        public class ReqInputModel
        {
            [Required, Display(Name = "Full Name")]
            public string FullName { get; set; }

            [Required, Display(Name = "Address")]
            public string Address { get; set; }

            [Required, Display(Name = "Password"), DataType(DataType.Password)]
            public string Password { get; set; }

            [Required, Display(Name = "ConfrimPassword"), Compare(nameof(Password), ErrorMessage = "The ConfrimPassword and Password do not Matched"), DataType(DataType.Password)]
            public string ConfrimPassword { get; set; }

            [EmailAddress, Required]
            public string Email { get; set; }

            public bool IsAgree { get; set; }
        }

        [BindProperty]
        public ReqInputModel ReqInput { get; set; }


        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                if (!ReqInput.IsAgree)
                {
                    ModelState.AddModelError(string.Empty, "You must agree on the website rules and terms.");
                    return Page();
                }

                UserApplication user = new UserApplication { UserName = new MailAddress(ReqInput.Email).User, FullName = ReqInput.FullName, Email = ReqInput.Email, Address = ReqInput.Address };

                var resault = await _userManager.CreateAsync(user, ReqInput.Password);

                if (resault.Succeeded)
                {
                    _logger.LogInformation("User Created a new Account");

                    var userid = await _userManager.GetUserIdAsync(user);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var url = Url.Page("/Accounts/ConfrimEmail",
                        pageHandler: null,
                        values: new { area = "Portal", userId = userid, code = code },
                        protocol: Request.Scheme
                        );

                    await appEmailService.SendEmailAsync(user.Email, "Confrim Your Email",
                 $"Please Confrim your email by <a href='{HtmlEncoder.Default.Encode(url)}'>Clicking here</a>"
                           );
                    if (_userManager.Options.SignIn.RequireConfirmedEmail)
                    {
                        return RedirectToPage("RegisterConfrim", new { email = ReqInput.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                    }


                    return RedirectToPage("/Index");
                }

                foreach (var item in resault.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                    return Page();
                }
            }

            return Page();
        }

    }
}