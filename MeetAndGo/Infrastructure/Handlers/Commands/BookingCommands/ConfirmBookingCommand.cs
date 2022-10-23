using System.Threading.Tasks;
using MeetAndGo.Data;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Extensions;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Infrastructure.Services.Email;
using MeetAndGo.Infrastructure.Utils;
using MeetAndGo.Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;

namespace MeetAndGo.Infrastructure.Handlers.Commands.BookingCommands
{
    public class ConfirmBookingCommand : ICommand
    {
        public int BookingId { get; }

        public ConfirmBookingCommand(int bookingId)
        {
            BookingId = bookingId;
        }
    }

    public sealed class ConfirmBookingCommandHandler : ICommandHandler<ConfirmBookingCommand>
    {
        private readonly MeetGoDbContext _dbContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IValidationService _validationService;
        private readonly IMailService _mailService;

        public ConfirmBookingCommandHandler(MeetGoDbContext dbContext,
            IIdentityProvider identityProvider,
            IValidationService validationService,
            IMailService mailService)
        {
            _dbContext = dbContext;
            _identityProvider = identityProvider;
            _validationService = validationService;
            _mailService = mailService;
        }

        public async Task<Result> Handle(ConfirmBookingCommand command)
        {
            var validation = _validationService.ValidateRules(new ConfirmBookingCommandValidator(), command);
            if (validation.IsFailure) return Result.Fail(validation.Error);

            var companyUserId = _identityProvider.GetUserIdFromClaims();
            var booking = await GetBooking(command);

            if (booking.Visit.Event.UserId != companyUserId)
                return Result.Fail($"Booking with id {booking.Id} does not belong to the user {companyUserId}");

            booking.IsConfirmed = true;

            await _dbContext.SaveChangesAsync();

            _ = NotifyCustomer(booking);

            return Result.Ok();
        }

        private async Task<Booking> GetBooking(ConfirmBookingCommand command) =>
            await _dbContext.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Visit)
                    .ThenInclude(v => v.Event)
                .SingleOrDefaultAsync(b => b.Id == command.BookingId);

        private async Task NotifyCustomer(Booking booking)
        {
            var visitDate = booking.Visit.StartDate.ToFriendlyString();
            var eventName = booking.Visit.Event.Name;
            var companyMailRequest = new MailRequest
            {
                Subject = $"Potwierdzono rezerwację: {eventName}, {visitDate}",
                ToEmail = $"{booking.Customer.Email}",
                Body = $"<div style=\"font-size: 16px;\"><p>Dzień dobry,</p><p>z przyjemnością informujemy, że wydarzenie {eventName} w dniu {visitDate} zostało potwierdzone przez gospodarza." +
                       "<p>Życzymy miłej zabawy :)</p>" +
                       "<p>Wszystkie swoje nadchodzące rezerwacje możesz zobaczyć po zalogowaniu na stronie meetgo w zakładce &quot;Rezerwacje&quot;.</p></div>"
            };

            await _mailService.SendEmailAsync(companyMailRequest);
        }
    }
}