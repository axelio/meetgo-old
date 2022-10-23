using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MeetAndGo.Data;
using MeetAndGo.Data.Dto;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Extensions;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Infrastructure.Utils;
using MeetAndGo.Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;

namespace MeetAndGo.Infrastructure.Handlers.Queries.EventQueries
{
    public class GetEventsQuery : IQuery
    {
        public DateTime Day { get; set; }
        public int CityId { get; set; }
        public int? LastVisitId { get; set; }
        public int? CategoryId { get; set; }
        public int? TimeOfDay { get; set; }
    }

    public sealed class GetEventsQueryHandler : IQueryHandler<GetEventsQuery, Result<GetEventsQueryResult>>
    {
        private readonly IMapper _mapper;
        private readonly MeetGoDbContext _dbContext;
        private readonly IValidationService _validationService;

        public GetEventsQueryHandler(IMapper mapper, MeetGoDbContext dbContext, IValidationService validationService)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _validationService = validationService;
        }

        public async Task<Result<GetEventsQueryResult>> Handle(GetEventsQuery query)
        {
            var validationResult = _validationService.ValidateRules(new GetEventsQueryValidator(), query);
            if (validationResult.IsFailure) return Result.Fail<GetEventsQueryResult>(validationResult.Error);

            var result = await GetEvents(query);
            return Result.Ok(result);
        }

        private async Task<GetEventsQueryResult> GetEvents(GetEventsQuery query)
        {
            var dbQuery = PrepareDbQuery(query);

            var visits = await _mapper.ProjectTo<VisitWithEventDto>(dbQuery).ToListAsync();
            if (!visits.Any()) return new GetEventsQueryResult();
            var eventsByIds = PrepareEventsWithVisits(query, visits);

            return new GetEventsQueryResult
            {
                Events = eventsByIds,
                LastVisitId = visits.Last().Id,
                VisitsCount = visits.Count
            };
        }

        private Dictionary<int, EventWithVisitsDto> PrepareEventsWithVisits(GetEventsQuery query, List<VisitWithEventDto> visits)
        {
            var random = new Random(query.Day.DayOfYear);

            return visits.Aggregate(new Dictionary<int, EventWithVisitsDto>(), (seed, visit) =>
            {
                if (!seed.ContainsKey(visit.Event.Id))
                {
                    var newEvent = _mapper.Map<EventWithVisitsDto>(visit.Event);
                    newEvent.Visits = new List<VisitDisplayDto>();
                    newEvent.Order = random.Next(100);
                    seed.Add(visit.Event.Id, newEvent);
                }
                seed[visit.Event.Id].Visits.Add(_mapper.Map<VisitDisplayDto>(visit));

                return seed;
            });
        }

        private IQueryable<Visit> PrepareDbQuery(GetEventsQuery query)
        {
            //Todo 1: RAW SQL
            //Todo 2: Set up indexes
            return _dbContext.Visits
                .AsNoTracking()
                .Where(v => !v.IsBooked)
                .Where(v => v.CityId == query.CityId)
                .ApplyDateCondition(query.Day)
                .ApplyLastIdCondition(query.LastVisitId)
                .ApplyTimeOfDayCondition(query.TimeOfDay)
                .ApplyCategoryIdCondition(query.CategoryId)
                .Take(30);
        }
    }
}
