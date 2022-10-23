using FluentValidation.TestHelper;
using MeetAndGo.Infrastructure.Handlers.Queries.EventQueries;
using MeetAndGo.Infrastructure.Validators;
using System;
using Xunit;

namespace MeetAndGo.Tests.Validation
{
    public class GetEventsQueryValidatorTests
    {
        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public void ShouldHaveErrorForWrongCityId(int cityId)
        {
            var validator = new GetEventsQueryValidator();
            var model = GetValidSimpleQuery();
            model.CityId = cityId;
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(e => e.CityId);
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public void ShouldHaveErrorForWrongCategoryId(int categoryId)
        {
            var validator = new GetEventsQueryValidator();
            var model = GetValidSimpleQuery();
            model.CategoryId = categoryId;
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(e => e.CategoryId);
        }

        [Theory]
        [InlineData(4)]
        [InlineData(0)]
        public void ShouldHaveErrorForWrongTimeOfDay(int timeOfDay)
        {
            var validator = new GetEventsQueryValidator();
            var model = GetValidSimpleQuery();
            model.TimeOfDay = timeOfDay;
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(e => e.TimeOfDay);
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public void ShouldHaveErrorForWrongLastVisitId(int visitId)
        {
            var validator = new GetEventsQueryValidator();
            var model = GetValidSimpleQuery();
            model.LastVisitId = visitId;
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(e => e.LastVisitId);
        }

        [Fact]
        public void ShouldNotHaveErrorsForSimpleQuery()
        {
            var validator = new GetEventsQueryValidator();
            var model = GetValidSimpleQuery();
            var result = validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void ShouldNotHaveErrorsForFullQuery()
        {
            var validator = new GetEventsQueryValidator();
            var model = GetValidFullQuery();
            var result = validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        private static GetEventsQuery GetValidSimpleQuery()
        {
            return new GetEventsQuery
            {
                Day = new DateTime(2030, 12, 10),
                CityId = 10,
            };
        }

        private static GetEventsQuery GetValidFullQuery()
        {
            return new GetEventsQuery
            {
                Day = new DateTime(2030, 12, 10),
                CityId = 10,
                LastVisitId = 300,
                CategoryId = 3,
                TimeOfDay = 1
            };
        }
    }
}
