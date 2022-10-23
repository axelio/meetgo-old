using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace MeetAndGo.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IMailService _mailService;

        public ResendEmailConfirmationModel(UserManager<User> userManager, IMailService mailService)
        {
            _userManager = userManager;
            _mailService = mailService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Strona nie wymaga potwierdzenia e-mail. Wiadomość nie została wysłana.");
                return Page();
            }

            //var userId = await _userManager.GetUserIdAsync(user);
            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //var callbackUrl = Url.Page(
            //    "/Account/ConfirmEmail",
            //    pageHandler: null,
            //    values: new { userId = userId, code = code },
            //    protocol: Request.Scheme);

            //var mail = new MailRequest
            //{
            //    Body = "<div style=\"font-size: 16px;\"><p>Dzień dobry.</p>" +
            //           "<p>To kolejna wiadomość z linkiem aktywacyjnym wysłana na Twoją prośbę</p>" +
            //           "<p>Dziękujęmy za rejestrację w meetgo. Aby móc używać swojego konta i dokonywać rezerwacji konieczne jest potwierdzenie adresu e-mail.</p>" +
            //           $"<p>Możesz to zrobić klikając <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>w ten link</a></p>" +
            //           $"<p>Jeżeli link nie działa spróbuj skopiować go do przeglądarki: {HtmlEncoder.Default.Encode(callbackUrl)}</p></div>",
            //    Subject = "meetgo - potwierdź swój adres e-mail",
            //    ToEmail = Input.Email
            //};

            //_ = _mailService.SendEmailAsync(mail);

            ModelState.AddModelError(string.Empty, "Strona nie wymaga potwierdzenia e-mail. Wiadomość nie została wysłana.");
            return Page();
        }
    }
}
