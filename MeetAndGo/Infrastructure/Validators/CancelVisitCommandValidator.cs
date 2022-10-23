using FluentValidation;
using MeetAndGo.Infrastructure.Handlers.Commands.VisitCommands;

namespace MeetAndGo.Infrastructure.Validators
{
    public class CancelVisitCommandValidator : AbstractValidator<CancelVisitCommand>
    {
        public CancelVisitCommandValidator()
        {
            RuleFor(x => x.VisitId).GreaterThan(0);

        }
    }
}