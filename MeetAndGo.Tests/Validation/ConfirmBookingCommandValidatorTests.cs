using FluentValidation.TestHelper;
using MeetAndGo.Infrastructure.Handlers.Commands.BookingCommands;
using MeetAndGo.Infrastructure.Validators;
using Xunit;

namespace MeetAndGo.Tests.Validation
{
    public class ConfirmBookingCommandValidatorTests
    {
        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public void ShouldHaveErrorWhenWrongBookingId(int id)
        {
            var validator = new ConfirmBookingCommandValidator();
            var model = new ConfirmBookingCommand(id);
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(e => e.BookingId);
        }

        [Fact]
        public void ShouldNotHaveErrors()
        {
            var validator = new ConfirmBookingCommandValidator();
            var model = new ConfirmBookingCommand(300);
            var result = validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
