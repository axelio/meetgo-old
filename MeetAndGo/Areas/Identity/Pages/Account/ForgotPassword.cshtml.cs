using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
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
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IMailService _mailService;

        public ForgotPasswordModel(UserManager<User> userManager, IMailService mailService)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                var mail = new MailRequest
                {
                    Body = "<div style=\"font-size: 16px;\"><p>Dzień dobry.</p>" +
                           "<p>W związku z prośbą o zresetowanie hasła przesyłamy poniżej link, którego należy użyć w celu procedury zmiany hasła.</p>"+
                           "<p>Jeżeli nie wysyłałeś takiej prośby skontaktuj się z nami.</p>" +
                           $"<p>Aby zresetować swoje hasło kliknij <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>w ten link</a>.</p>" +
                           $"<p>Jeżeli link nie działa spróbuj skopiować go do przeglądarki: {HtmlEncoder.Default.Encode(callbackUrl)}</p></div>",
                    Subject = "meetgo - zrestartuj swoje hasło.",
                    ToEmail = Input.Email
                };

                _ = _mailService.SendEmailAsync(mail);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
