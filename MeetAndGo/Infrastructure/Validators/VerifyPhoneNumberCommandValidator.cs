using FluentValidation;
using MeetAndGo.Infrastructure.Handlers.Commands.PhoneNumberCommands;

namespace MeetAndGo.Infrastructure.Validators
{
    public class VerifyPhoneNumberCommandValidator : AbstractValidator<VerifyPhoneNumberCommand>
    {
        public VerifyPhoneNumberCommandValidator()
        {
            RuleFor(x => x.Token).NotEmpty().NotNull();
        }
    }
}