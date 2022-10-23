using System;
using System.Linq;
using System.Threading.Tasks;
using MeetAndGo.Data;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Extensions;
using MeetAndGo.Infrastructure.Helpers;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Infrastructure.Services.Email;
using MeetAndGo.Infrastructure.Utils;
using MeetAndGo.Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;

namespace MeetAndGo.Infrastructure.Handlers.Commands.BookingCommands
{
    public class MakeBookingCommand : ICommand
    {
        public int VisitId { get; }

        public MakeBookingCommand(int visitId)
        {
            VisitId = visitId;
        }
    }

    public sealed class MakeBookingCommandCommandHandler : ICommandHandler<MakeBookingCommand>
    {
        private readonly MeetGoDbContext _dbContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IValidationService _validationService;
        private readonly IMailService _mailService;
        private readonly Random _random = new();

        public MakeBookingCommandCommandHandler(
            MeetGoDbContext dbContext,
            IIdentityProvider identityProvider,
            IValidationService validationService,
            IMailService mailService)
        {
            _dbContext = dbContext;
            _identityProvider = identityProvider;
            _validationService = validationService;
            _mailService = mailService;
        }

        public async Task<Result> Handle(MakeBookingCommand command)
        {
            var validation = _validationService.ValidateRules(new MakeBookingCommandValidator(), command);
            if (validation.IsFailure) return Result.Fail(validation.Error);

            var customer = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == _identityProvider.GetUserIdFromClaims());

            var visit = await GetVisit(command);

            var isPossibleResult = await IsBookingPossible(visit, customer);
            if (isPossibleResult.IsFailure)
                return Result.Fail(isPossibleResult.Error);

            var newBooking = CreateBooking(customer, visit);
            _dbContext.Bookings.Add(newBooking);
            SetVisitAsBooked(visit);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Fail(MakeBookingError.AlreadyBooked);
            }

            NotifyParties(customer, visit, newBooking.Code);

            return Result.Ok();
        }

        private string GenerateBookingCode() => _random.Next(0, 9999).ToString("D4");

        private static void SetVisitAsBooked(Visit visit)
        {
            visit.IncrementBookingsNumber();

            var isBooking = visit.Event.Kind == EventKind.Booking;
            var isEnrollmentKindAndFull = visit.Event.Kind == EventKind.Enrollment && visit.BookingsNumber == visit.MaxPersons;

            if (isBooking || isEnrollmentKindAndFull)
                visit.SetAsBooked();
        }

        private void NotifyParties(User customer, Visit visit, string bookingCode)
        {
            _ = NotifyCompany(visit, customer);

            if (!visit.Event.RequiresConfirmation)
                _ = NotifyCustomer(visit, customer, bookingCode);
        }

        private async Task<Result> IsBookingPossible(Visit visit, User customer)
        {
            if (visit.IsBooked)
                return Result.Fail(MakeBookingError.AlreadyBooked);

            if (string.IsNullOrEmpty(customer.PhoneNumber))
                return Result.Fail(MakeBookingError.NoPhoneNumber);

            if (!customer.PhoneNumberConfirmed)
                return Result.Fail(MakeBookingError.PhoneNumberNotVerified);

            if (!await CanBookMore(customer, visit))
                return Result.Fail(MakeBookingError.TooManyBookings);

            return Result.Ok();
        }

        private async Task<Visit> GetVisit(MakeBookingCommand command) =>
            await _dbContext.Visits
                .AsTracking()
                .Include(v => v.Event)
                    .ThenInclude(v => v.Address).ThenInclude(a => a.City)
                .Include(v => v.Event)
                    .ThenInclude(e => e.Company)
                .SingleOrDefaultAsync(v => v.Id == command.VisitId);

        private async Task<bool> CanBookMore(User customer, Visit visit)
        {
            const int maxBookingsPerCustomer = 2;

            var bookings = await _dbContext.Bookings
                .AsNoTracking()
                .Where(b => b.Customer.PhoneNumber == customer.PhoneNumber)
                .Where(b => b.Visit.StartDate.Date == visit.StartDate.Date)
                .CountAsync();

            return bookings < maxBookingsPerCustomer;
        }

        private Booking CreateBooking(User customer, Visit visit) =>
            new()
            {
                Customer = customer,
                Visit = visit,
                IsConfirmed = !visit.Event.RequiresConfirmation,
                Code = GenerateBookingCode()
            };

        private async Task NotifyCustomer(Visit visit, User customer, string bookingCode)
        {
            var visitDate = visit.StartDate.ToFriendlyString();
            var eventName = visit.Event.Name;
            var companyMailRequest = new MailRequest
            {
                Subject = $"Potwierdzono rezerwację: {eventName}, {visitDate}",
                ToEmail = $"{customer.Email}",
                Body = $"<div style=\"font-size: 16px;\"><p>Dzień dobry,</p><p>z przyjemnością informujemy, że wydarzenie {eventName} w dniu {visitDate} zostało automatycznie potwierdzone przez gospodarza." +
                       $"<p>Twój kod rezerwacji to: {bookingCode}</p>" +
                       "<p>Życzymy miłej zabawy :)</p>" +
                       "<p>Wszystkie swoje nadchodzące rezerwacje możesz zobaczyć po zalogowaniu na stronie meetgo w zakładce &quot;Rezerwacje&quot;.</p></div>"
            };

            await _mailService.SendEmailAsync(companyMailRequest);
        }

        private async Task NotifyCompany(Visit visit, User customer)
        {
            var visitDate = visit.StartDate.ToFriendlyString();

            var msgBody =
                $"<div style=\"font-size: 16px;\"><p>Dzień dobry!<br><br>Z przyjemnością informujemy, że dokonano nowej rezerwacji na Wasze wydarzenie. Poniżej znajdują się informacje o wydarzeniu oraz dane rezerwującego. <br>" +
                $"Wszystkie swoje wydarzenia wraz z ich rezerwacjami mogą Państwo zobaczyć po zalogowaniu na stronie meetgo w zakładce &quot;wydarzenia&quot;.</p>" +
                $"<p>Nazwa wydarzenia: {visit.Event.Name}<br>" +
                $"Data: {visitDate}<br>" +
                $"Cena: {visit.Price} PLN<br>" +
                $"Maksymalna liczba osób: {visit.MaxPersons}<br>" +
                $"Adres e-mail rezerwującego: <strong>{customer.Email}</strong><br>" +
                $"Numer telefonu rezerwującego: {customer.PhoneNumber}</p>";


            msgBody += visit.Event.RequiresConfirmation
                ? "<p><strong>UWAGA! To wydarzenie zgodnie z Państwa preferencją wymaga potwierdzenia. " +
                  "Można to zrobić stronie meetgo w zakładce Wydarzenia</strong></p></div>"
                : "</div>";

            var mailRequest = new MailRequest
            {
                Subject = $"Nowa rezerwacja: {visit.Event.Name}, {visitDate}",
                ToEmail = $"{visit.Event.Company.Email}",
                Body = msgBody
            };

            await _mailService.SendEmailAsync(mailRequest);
        }

        private static class MakeBookingError
        {
            public static string AlreadyBooked = "ALREADY_BOOKED";
            public static string TooManyBookings = "TOO_MANY_BOOKINGS";
            public static string NoPhoneNumber = "NO_PHONE_NUMBER";
            public static string PhoneNumberNotVerified = "PHONE_NUMBER_NOT_VERIFIED";
        }
    }
}
