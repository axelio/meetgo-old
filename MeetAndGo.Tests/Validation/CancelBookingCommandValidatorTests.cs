using FluentValidation.TestHelper;
using MeetAndGo.Infrastructure.Handlers.Commands.BookingCommands;
using MeetAndGo.Infrastructure.Validators;
using Xunit;

namespace MeetAndGo.Tests.Validation
{

    public class CancelBookingCommandValidatorTests
    {
        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public void ShouldHaveErrorWhenWrongBookingId(int id)
        {
            var validator = new CancelBookingCommandValidator();
            var model = new CancelBookingCommand(id);
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(e => e.BookingId);
        }

        [Fact]
        public void ShouldNotHaveErrors()
        {
            var validator = new CancelBookingCommandValidator();
            var model = new CancelBookingCommand(300);
            var result = validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
