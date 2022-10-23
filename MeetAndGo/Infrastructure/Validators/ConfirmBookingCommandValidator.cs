using FluentValidation;
using MeetAndGo.Infrastructure.Handlers.Commands.BookingCommands;

namespace MeetAndGo.Infrastructure.Validators
{
    public class ConfirmBookingCommandValidator : AbstractValidator<ConfirmBookingCommand>
    {
        public ConfirmBookingCommandValidator()
        {
            RuleFor(x => x.BookingId).GreaterThan(0);
        }
    }
}