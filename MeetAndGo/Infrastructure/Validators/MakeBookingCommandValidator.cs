using FluentValidation;
using MeetAndGo.Infrastructure.Handlers.Commands.BookingCommands;

namespace MeetAndGo.Infrastructure.Validators
{
    public class MakeBookingCommandValidator : AbstractValidator<MakeBookingCommand>
    {
        public MakeBookingCommandValidator()
        {
            RuleFor(x => x.VisitId).GreaterThan(0);
        }
    }
}