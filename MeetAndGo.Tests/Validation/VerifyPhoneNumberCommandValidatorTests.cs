using FluentValidation.TestHelper;
using MeetAndGo.Infrastructure.Handlers.Commands.PhoneNumberCommands;
using MeetAndGo.Infrastructure.Validators;
using Xunit;

namespace MeetAndGo.Tests.Validation
{
    public class VerifyPhoneNumberCommandValidatorTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void ShouldHaveErrorWhenWrongToken(string token)
        {
            var validator = new VerifyPhoneNumberCommandValidator();
            var model = new VerifyPhoneNumberCommand { Token = token };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(e => e.Token);
        }

        [Fact]
        public void ShouldNotHaveErrors()
        {
            var validator = new VerifyPhoneNumberCommandValidator();
            var model = new VerifyPhoneNumberCommand { Token = "this_is_33232" };
            var result = validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
