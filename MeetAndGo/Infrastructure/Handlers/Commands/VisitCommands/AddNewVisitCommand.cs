using System;
using System.Threading.Tasks;
using MeetAndGo.Data;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Helpers;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Infrastructure.Utils;
using MeetAndGo.Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;

namespace MeetAndGo.Infrastructure.Handlers.Commands.VisitCommands
{
    public class AddNewVisitCommand : ICommand
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public int MaxPersons { get; set; }
        public int EventId { get; set; }
    }

    public class AddNewVisitCommandHandler : ICommandHandler<AddNewVisitCommand>
    {
        private readonly MeetGoDbContext _dbContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IValidationService _validationService;

        public AddNewVisitCommandHandler(
            MeetGoDbContext dbContext,
            IIdentityProvider identityProvider,
            IValidationService validationService)
        {
            _dbContext = dbContext;
            _identityProvider = identityProvider;
            _validationService = validationService;
        }
        public async Task<Result> Handle(AddNewVisitCommand command)
        {
            var validation = _validationService.ValidateRules(new AddNewVisitCommandValidator(), command);
            if (validation.IsFailure) return Result.Fail(validation.Error);

            var companyId = _identityProvider.GetUserIdFromClaims();

            var eventEntity = await GetEvent(command.EventId);
            if (eventEntity.UserId != companyId)
                return Result.Fail($"Event: {command.EventId} does not belong to the user {companyId}");

            var canAddValidation = await CheckCanAdd(companyId, command);
            if (canAddValidation.IsFailure) return Result.Fail(canAddValidation.Error);

            _dbContext.Visits.Add(PrepareVisit(command, eventEntity));
            await _dbContext.SaveChangesAsync();

            return Result.Ok();
        }

        private static Visit PrepareVisit(AddNewVisitCommand command, Event e)
        {
            var offset = DateTimeOffset.Now.Offset;
            DateTimeOffset PrepareStartDate(DateTime day) => new(day, offset);

            return new Visit
            {
                StartDate = PrepareStartDate(command.Date),
                EventId = e.Id,
                MaxPersons = command.MaxPersons,
                CityId = e.Address.CityId,
                Price = command.Price,
                TimeOfDay = GetTimeOfDay(command.Date)
            };
        }

        private async Task<Result> CheckCanAdd(string companyId, AddNewVisitCommand command)
        {
            var companySettings = await _dbContext.CompanySettings.AsNoTracking().FirstOrDefaultAsync(cs => cs.UserId == companyId);
            var todayVisits = await _dbContext.Visits.AsNoTracking().CountAsync(v => v.Event.UserId == companyId && v.StartDate.Date == command.Date.Date);

            if (!companySettings.IsActive) return Result.Fail(AddNewVisitsError.NotActive);
            if (todayVisits + 1 > companySettings.MaxDailyVisits) return Result.Fail(AddNewVisitsError.TooMany);

            return Result.Ok();
        }

        private static int GetTimeOfDay(DateTime date)
        {
            return date.Hour switch
            {
                < 12 => TimeOfDay.Morning,
                < 18 => TimeOfDay.Afternoon,
                _ => TimeOfDay.Evening
            };
        }

        private async Task<Event> GetEvent(int eventId) =>
            await _dbContext.Events
                .AsNoTracking()
                .Include(c => c.Address)
                .FirstOrDefaultAsync(ev => ev.Id == eventId);

        private static class AddNewVisitsError
        {
            public static string NotActive => "NOT_ACTIVE";
            public static string TooMany => "TOO_MANY";
        }
    }
}