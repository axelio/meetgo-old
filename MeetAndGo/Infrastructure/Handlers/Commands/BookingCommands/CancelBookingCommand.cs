using System.Threading.Tasks;
using AutoMapper;
using MeetAndGo.Data;
using MeetAndGo.Data.Dto;
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
    public class CancelBookingCommand : ICommand
    {
        public int BookingId { get; }

        public CancelBookingCommand(int bookingId) => BookingId = bookingId;
    }

    public sealed class CancelBookingCommandHandler : ICommandHandler<CancelBookingCommand>
    {
        private readonly MeetGoDbContext _dbContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IValidationService _validationService;
        private readonly IMailService _mailService;
        private readonly IDeletedEntitiesService _deletedEntitiesService;
        private readonly IMapper _mapper;

        public CancelBookingCommandHandler(
            MeetGoDbContext dbContext,
            IIdentityProvider identityProvider,
            IValidationService validationService,
            IMailService mailService,
            IDeletedEntitiesService deletedEntitiesService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _identityProvider = identityProvider;
            _validationService = validationService;
            _mailService = mailService;
            _deletedEntitiesService = deletedEntitiesService;
            _mapper = mapper;
        }

        public async Task<Result> Handle(CancelBookingCommand command)
        {
            var validation = _validationService.ValidateRules(new CancelBookingCommandValidator(), command);
            if (validation.IsFailure) return Result.Fail(validation.Error);

            var customerUserId = _identityProvider.GetUserIdFromClaims();
            var booking = await GetBooking(command.BookingId);

            if (booking.UserId != customerUserId)
                return Result.Fail($"Booking: {booking.Id} does not belong to {customerUserId}");
            
            StoreDeletedEntity(booking);
            _dbContext.Remove(booking);
            booking.Visit.DecrementBookingsNumber();
            booking.Visit.SetAsNotBooked();

            await _dbContext.SaveChangesAsync();

            _ = NotifyCompany(booking);

            return Result.Ok();
        }

        private void StoreDeletedEntity(Booking booking)
        {
            _deletedEntitiesService.StoreDeletedEntity(new DeletedEntityDto
            {
                IdOrig = booking.Id,
                Type = DeletedEntityType.Booking,
                Obj = _mapper.Map<DeletedBookingDto>(booking)
            });
        }

        private async Task<Booking> GetBooking(int bookingId) => 
            await _dbContext.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Visit)
                .ThenInclude(v => v.Event)
                .ThenInclude(e => e.Company)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

        private async Task NotifyCompany(Booking booking)
        {
            var visitDate = booking.Visit.StartDate.ToFriendlyString();
            var companyMailRequest = new MailRequest
            {
                Subject = $"Anulowano rezerwację: {booking.Visit.Event.Name}, {visitDate}",
                ToEmail = $"{booking.Visit.Event.Company.Email}",
                Body = $"<div style=\"font-size: 16px;\"><p>Dzień dobry,</p><p>uprzejmie informujemy, że użytkownik {booking.Customer.UserName} anulował swoją rezerwację na wydarzenie {booking.Visit.Event.Name} w dniu {visitDate}.<br>" +
                      "Tym samym wydarzenie znowu jest widoczne na głównej stronie i może zostać zarezerwowane przez kogoś innego.</p></div>"
            };

            await _mailService.SendEmailAsync(companyMailRequest);
        }
    }

}