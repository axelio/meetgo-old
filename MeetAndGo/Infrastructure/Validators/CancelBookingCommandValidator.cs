using FluentValidation;
using MeetAndGo.Infrastructure.Handlers.Commands.BookingCommands;

namespace MeetAndGo.Infrastructure.Validators
{
    public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
    {
        public CancelBookingCommandValidator()
        {
            RuleFor(x => x.BookingId).GreaterThan(0);
        }
    }
}