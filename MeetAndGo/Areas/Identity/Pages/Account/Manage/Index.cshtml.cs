using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeetAndGo.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [StringLength(9, ErrorMessage = "Numer telefonu musi mieć dokładnie 9 znaków.", MinimumLength = 9)]
            [Display(Name = "Polski numer telefonu (wpisz bez +48)")]
            [RegularExpression("^[0-9]{9}$", ErrorMessage = "Numer telefonu musi mieć 9 cyfr - bez numeru kierunkowego")]
            public string PhoneNumber { get; set; }

            [StringLength(200, ErrorMessage = "Zbyt długie imię i nazwisko")]
            [Display(Name = "Imię")]
            public string Name { get; set; }
        }

        private void Load(User user)
        {
            var nameAndSurname = user.Name;

            Username = user.UserName;

            Input = new InputModel
            {
                PhoneNumber = string.IsNullOrEmpty(user.PhoneNumber) ? string.Empty : user.PhoneNumber.Replace("+48", ""),
                Name = string.IsNullOrEmpty(nameAndSurname) ? string.Empty :  user.Name
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Load(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                Load(user);
                return Page();
            }

            if (Input.PhoneNumber != user.PhoneNumber)
            {
                user.PhoneNumber = string.IsNullOrEmpty(Input.PhoneNumber)
                    ? Input.PhoneNumber
                    : PhoneNumberFactory.CreatePolishNumber(Input.PhoneNumber);
                user.PhoneNumberConfirmed = false;
            }

            if (Input.Name != user.Name)
                user.Name = Input.Name;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                StatusMessage = "Wystąpił niespodziewany błąd poczas próby zaaktualizowania danych.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Twój profil został zaaktualizowany";
            return RedirectToPage();
        }
    }
}
