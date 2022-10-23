using FluentValidation.TestHelper;
using MeetAndGo.Infrastructure.Handlers.Commands.BookingCommands;
using MeetAndGo.Infrastructure.Validators;
using Xunit;

namespace MeetAndGo.Tests.Validation
{
    public class MakeBookingCommandValidatorTests
    {
        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public void ShouldHaveErrorWhenWrongBookingId(int id)
        {
            var validator = new MakeBookingCommandValidator();
            var model = new MakeBookingCommand(id);
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(e => e.VisitId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void ShouldNotHaveErrors(int id)
        {
            var validator = new MakeBookingCommandValidator();
            var model = new MakeBookingCommand(id);
            var result = validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
