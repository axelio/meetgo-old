using FluentValidation.TestHelper;
using MeetAndGo.Infrastructure.Handlers.Commands.PhoneNumberCommands;
using MeetAndGo.Infrastructure.Validators;
using Xunit;

namespace MeetAndGo.Tests.Validation
{
    public class AddPhoneNumberCommandValidatorTests
    {
        [Theory]
        [InlineData("123fas")]
        [InlineData("-566")]
        [InlineData("72111122")]

        public void ShouldFailForWrongNumber(string number)
        {
            var validator = new AddPhoneNumberCommandValidator();
            var model = new AddPhoneNumberCommand
            {
                PhoneNumber = number
            };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(e => e.PhoneNumber);
        }

        [Fact]
        public void ShouldNotHaveErrors()
        {
            var validator = new AddPhoneNumberCommandValidator();
            var model = new AddPhoneNumberCommand
            {
                PhoneNumber = "796555111"
            };
            var result = validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
