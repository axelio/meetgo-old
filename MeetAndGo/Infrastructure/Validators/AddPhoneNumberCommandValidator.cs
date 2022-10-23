using FluentValidation;
using MeetAndGo.Infrastructure.Handlers.Commands.PhoneNumberCommands;

namespace MeetAndGo.Infrastructure.Validators
{
    public class AddPhoneNumberCommandValidator : AbstractValidator<AddPhoneNumberCommand>
    {
        public AddPhoneNumberCommandValidator()
        {
            RuleFor(x => x.PhoneNumber).Matches("^[0-9]{9}$").WithMessage("Must be 9 digits.");
        }
    }
}