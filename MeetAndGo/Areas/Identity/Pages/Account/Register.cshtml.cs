using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Services.Email;
using MeetAndGo.Infrastructure.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace MeetAndGo.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IMailService _mailService;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IMailService mailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mailService = mailService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "{0} musi mieć minimum {2} znaków.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potwierdź hasło")]
            [Compare("Password", ErrorMessage = "Hasła nie pasują.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new User { UserName = Input.Email, Email = Input.Email };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User: {user.Email} created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    var mail = new MailRequest
                    {
                        Body = "<div style=\"font-size: 16px;\"><p>Witamy w meetgo!</p>" +
                               "<p>Dziękujęmy za rejestrację. Twoje konto zostało poprawnie utworzone.</p>" +
                               "<p>W naszej aplikacji znajdziesz wiele interesujących wydarzeń w promocyjnych cenach dla Ciebie i Twoich znajomych.</p>" +
                               "<p>Pamiętaj, że:</p>" +
                               "<ul>" +
                               "<li>za rezerwację płacisz dopiero na miejscu,</li>" +
                               "<li>meetgo nie pobiera żadnych opłat prowizyjnych za dokonanie rezerwacji,</li>" +
                               "<li>podane ceny przy aktywności lub wydarzeniu to cena za wszystkie osoby,</li>" +
                               "<li>możesz dokonać maksymalnie dwie rezerwacje na jeden numer telefonu</li>" +
                               "</ul>" +
                               "<p>Przed dokonaniem Twojej pierwszej rezerwacji poprosimy Cię jednorazowo o zweryfikowanie Twojego numeru telefonu. " +
                               "Dzięki weryfikacji umożliwimy gospodarzowi rezerwacji kontakt z Tobą jeżeli zajdzie taka potrzeba oraz zapewniamy lepszą jakość działania aplikacji.</p>" +
                               "<p>Jeżeli chcesz dowiedzieć się więcej zapraszamy na stronę &quot;Jak to działa?&quot;, którą znajdziesz na głównej stronie.</p>" +
                               "</div>",
                        Subject = "Witamy w meetgo",
                        ToEmail = Input.Email
                    };

                    _ = _mailService.SendEmailAsync(mail);

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation");
                    //}
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
