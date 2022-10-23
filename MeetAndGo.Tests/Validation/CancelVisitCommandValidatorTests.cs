using FluentValidation.TestHelper;
using MeetAndGo.Infrastructure.Handlers.Commands.VisitCommands;
using MeetAndGo.Infrastructure.Validators;
using Xunit;

namespace MeetAndGo.Tests.Validation
{
    public class CancelVisitCommandValidatorTests
    {
        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public void ShouldHaveErrorWhenWrongVisitId(int id)
        {
            var validator = new CancelVisitCommandValidator();
            var model = new CancelVisitCommand(id);
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(e => e.VisitId);
        }

        [Fact]
        public void ShouldNotHaveErrors()
        {
            var validator = new CancelVisitCommandValidator();
            var model = new CancelVisitCommand(300);
            var result = validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
