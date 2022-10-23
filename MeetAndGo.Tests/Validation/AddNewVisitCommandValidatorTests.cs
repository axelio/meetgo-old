using FluentValidation.TestHelper;
using MeetAndGo.Infrastructure.Handlers.Commands.VisitCommands;
using MeetAndGo.Infrastructure.Validators;
using System;
using Xunit;

namespace MeetAndGo.Tests.Validation
{

    public class AddNewVisitCommandValidatorTests
    {
        [Fact]
        public void ShouldHaveErrorWhenWrongEventId()
        {
            var validator = new AddNewVisitCommandValidator();
            var model = CreateValidModel();
            model.EventId = 0;
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(e => e.EventId);
        }

        [Fact]
        public void ShouldHaveErrorForWrongPrecisionPrice()
        {
            var validator = new AddNewVisitCommandValidator();
            var model = CreateValidModel();
            model.Price = 13.3345m;
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(e => e.Price);
        }

        [Fact]
        public void ShouldHaveErrorForMinusPrice()
        {
            var validator = new AddNewVisitCommandValidator();
            var model = CreateValidModel();
            model.Price = -30;
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(e => e.Price);
        }

        [Fact]
        public void ShouldHaveErrorForWrongDate()
        {
            var validator = new AddNewVisitCommandValidator();
            var model = CreateValidModel();
            model.Date = new DateTime(2010, 10, 12);
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(e => e.Date);
        }

        [Fact]
        public void ShouldNotHaveErrors()
        {
            var validator = new AddNewVisitCommandValidator();
            var model = CreateValidModel();
            var result = validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        private static AddNewVisitCommand CreateValidModel()
        {
            return new AddNewVisitCommand
            {
                MaxPersons = 3,
                Date = new DateTime(2030, 10, 12),
                EventId = 3,
                Price = 30.2m
            };
        }
    }
}
