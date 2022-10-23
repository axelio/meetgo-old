using System;
using FluentValidation;
using MeetAndGo.Infrastructure.Handlers.Queries.EventQueries;

namespace MeetAndGo.Infrastructure.Validators
{

    public class GetEventsQueryValidator : AbstractValidator<GetEventsQuery>
    {
        public GetEventsQueryValidator()
        {
            RuleFor(x => x.CityId).GreaterThanOrEqualTo(1);
            RuleFor(x => x.Day.Date).GreaterThanOrEqualTo(DateTimeOffset.Now.Date)
                .WithMessage("Requested events date must be today or future dates.");
            RuleFor(x => x.LastVisitId).GreaterThanOrEqualTo(1);
            RuleFor(x => x.CategoryId).GreaterThanOrEqualTo(1);
            RuleFor(x => x.TimeOfDay).InclusiveBetween(1, 3);
        }
    }
}
