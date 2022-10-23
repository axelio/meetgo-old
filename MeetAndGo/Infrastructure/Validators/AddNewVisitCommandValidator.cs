using System;
using FluentValidation;
using MeetAndGo.Infrastructure.Handlers.Commands.VisitCommands;

namespace MeetAndGo.Infrastructure.Validators
{
    public class AddNewVisitCommandValidator : AbstractValidator<AddNewVisitCommand>
    {
        public AddNewVisitCommandValidator()
        {
            RuleFor(x => x.EventId).GreaterThanOrEqualTo(1);
            RuleFor(x => x.Price).ScalePrecision(2, 10).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Date).NotNull().GreaterThanOrEqualTo(DateTimeOffset.Now.DateTime);
        }
    }
}