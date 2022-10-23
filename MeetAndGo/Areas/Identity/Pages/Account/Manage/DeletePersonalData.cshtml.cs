using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MeetAndGo.Data;
using MeetAndGo.Data.Dto;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Extensions;
using MeetAndGo.Infrastructure.Helpers;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Infrastructure.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MeetAndGo.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;
        private readonly MeetGoDbContext _dbContext;
        private readonly IDeletedEntitiesService _deletedEntitiesService;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;

        public DeletePersonalDataModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            MeetGoDbContext dbContext,
            IDeletedEntitiesService deletedEntitiesService,
            IMailService mailService,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _dbContext = dbContext;
            _deletedEntitiesService = deletedEntitiesService;
            _mailService = mailService;
            _mapper = mapper;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Nieprawidłowe hasło.");
                    return Page();
                }
            }

            if (await CheckIfUserIsCompany(user.Id))
                throw new Exception($"Company id: {user.Id} tried to delete their account");
            var bookings = await GetBookings(user);

            StoreDeletedBookings(bookings);
            UpdateVisits(bookings);

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");

            _ = NotifyCompanies(bookings, user.UserName);

            await _signInManager.SignOutAsync();

            _logger.LogWarning("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }

        private static void UpdateVisits(List<Booking> bookings)
        {
            foreach (var b in bookings)
            {
                b.Visit.DecrementBookingsNumber();
            }
        }

        private async Task<List<Booking>> GetBookings(User user)
        {
            // fetch needed for cascade delete - ef core rule
            return await _dbContext.Bookings
                .Include(b => b.Visit)
                    .ThenInclude(v => v.Event)
                    .ThenInclude(e => e.Company)
                .Where(b => b.UserId == user.Id)
                .ToListAsync();
        }

        private void StoreDeletedBookings(List<Booking> bookings)
        {
            if (!bookings.Any()) return;

            var deletedBookings = _mapper.Map<List<DeletedBookingDto>>(bookings).Select(b => new DeletedEntityDto
            {
                Obj = b,
                IdOrig = b.Id,
                Type = DeletedEntityType.Booking
            }).ToList();

            _deletedEntitiesService.StoreDeletedEntities(deletedBookings);
        }

        private async Task NotifyCompanies(List<Booking> bookings, string customerName)
        {
            if (!bookings.Any()) return;

            var bookingsByCompany = new Dictionary<string, List<Booking>>();
            foreach (var booking in bookings)
            {
                if (bookingsByCompany.ContainsKey(booking.Visit.Event.Company.Email))
                    bookingsByCompany[booking.Visit.Event.Company.Email].Add(booking);
                else
                    bookingsByCompany.Add(booking.Visit.Event.Company.Email, new List<Booking> { booking });
            }

            var tasks = bookingsByCompany.Select(kvp => NotifyCompany(kvp.Value, customerName, kvp.Key));
            await Task.WhenAll(tasks);
        }

        private async Task NotifyCompany(List<Booking> bookings, string customerName, string companyMail)
        {
            var companyMailRequest = new MailRequest
            {
                Subject = $"Anulowane rezerwacje użytkownika: {customerName}",
                ToEmail = $"{companyMail}",
                Body = $"<div style=\"font-size: 16px;\">" +
                       $"<p>Dzień dobry,</p><p>uprzejmie informujemy, że użytkownik {customerName} usunął swoje konto z portalu meetgo. Tym samym następujące rezerewacje u Państwa zostały anulowane. " +
                       $"Terminy wydarzeń wróciły do puli i mogą zostać zarezerwowane przez kogoś innego.</p>" +
                       $"<p>Anulowane rezerwacje:</p>" +
                       "<ul>" +
                       $"{string.Join(' ', bookings.Select(b => $"<li>{b.Visit.Event.Name}. {b.Visit.StartDate.ToFriendlyString()}</li>"))}" +
                       "</ul>" +
                       "</div>"
            };

            await _mailService.SendEmailAsync(companyMailRequest);
        }

        private async Task<bool> CheckIfUserIsCompany(string userId)
        {
            var claim = await _dbContext.UserClaims.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId && c.ClaimType == "Company");
            return claim is { ClaimValue: "true" };
        }
    }
}
