using System;
using System.Collections.Generic;
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

namespace MeetAndGo.Infrastructure.Handlers.Commands.VisitCommands
{
    public class CancelVisitCommand : ICommand
    {
        public int VisitId { get; }

        public CancelVisitCommand(int visitId)
        {
            VisitId = visitId;
        }
    }

    public sealed class CancelVisitCommandHandler : ICommandHandler<CancelVisitCommand>
    {
        private readonly MeetGoDbContext _dbContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IValidationService _validationService;
        private readonly IMailService _mailService;
        private readonly IDeletedEntitiesService _deletedEntitiesService;
        private readonly IMapper _mapper;

        public CancelVisitCommandHandler(
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
        public async Task<Result> Handle(CancelVisitCommand command)
        {
            var validation = _validationService.ValidateRules(new CancelVisitCommandValidator(), command);
            if (validation.IsFailure) return Result.Fail(validation.Error);

            var companyUserId = _identityProvider.GetUserIdFromClaims();

            var visit = await GetVisit(command.VisitId);

            if (visit.Event.UserId != companyUserId)
                return Result.Fail($"Visit: {visit.Id} does not belong to {companyUserId}");

            if (visit.StartDate <= DateTimeOffset.Now)
                return Result.Fail("Visit can not be cancelled. TOO_LATE");

            if (!visit.Bookings.IsNullOrEmpty())
                _dbContext.RemoveRange(visit.Bookings);

            _dbContext.Remove(visit);

            StoreDeletedEntities(visit);

            await _dbContext.SaveChangesAsync();

            if (!visit.Bookings.IsNullOrEmpty())
                _ = NotifyCustomer(visit);

            return Result.Ok();
        }

        private async Task<Visit> GetVisit(int visitId)
        {
            return await _dbContext.Visits
                .Include(v => v.Event)
                    .ThenInclude(e => e.Company)
                .Include(v => v.Event)
                    .ThenInclude(u => u.Address)
                .Include(v => v.Bookings)
                    .ThenInclude(b => b.Customer)
                .FirstOrDefaultAsync(v => v.Id == visitId);
        }

        private void StoreDeletedEntities(Visit visit)
        {
            StoreDeletedVisit(visit);

            if (!visit.Bookings.IsNullOrEmpty())
                foreach (var b in visit.Bookings)
                    StoreDeletedBooking(b);
        }

        private void StoreDeletedBooking(Booking b) => _deletedEntitiesService.StoreDeletedEntity(new DeletedEntityDto
        {
            IdOrig = b.Id,
            Type = DeletedEntityType.Booking,
            Obj = _mapper.Map<DeletedBookingDto>(b)
        });

        private void StoreDeletedVisit(Visit visit) => _deletedEntitiesService.StoreDeletedEntity(new DeletedEntityDto
        {
            IdOrig = visit.Id,
            Type = DeletedEntityType.Visit,
            Obj = _mapper.Map<DeletedVisitDto>(visit)
        });

        private async Task NotifyCustomer(Visit visit)
        {
            var visitDate = visit.StartDate.ToFriendlyString();

            var tasks = new List<Task>();
            foreach (var booking in visit.Bookings)
            {
                var companyMailRequest = new MailRequest
                {
                    Subject = $"Anulowano rezerwację: {visit.Event.Name}, {visitDate}",
                    ToEmail = $"{booking.Customer.Email}",
                    Body = $"<div style=\"font-size: 16px;\"><p>Dzień dobry,</p><p>z przykrością informujemy, że wydarzenie {visit.Event.Name} w dniu {visitDate} zostało anulowane przez {visit.Event.Address.CompanyName}. " +
                       "Tym samym niestety Twoja rezerwacja również musi zostać anulowana.</p>" +
                       "<p>Wszystkie swoje nadchodzące rezerwacje możesz zobaczyć po zalogowaniu na stronie meetgo w zakładce &quot;Rezerwacje&quot;.</p></div>"
                };

                tasks.Add(_mailService.SendEmailAsync(companyMailRequest));
            }

            await Task.WhenAll(tasks);
        }
    }
}