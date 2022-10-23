using System;
using System.Threading.Tasks;
using FluentAssertions;
using MeetAndGo.Infrastructure.Handlers.Queries.EventQueries;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Tests.Config;
using Xunit;

namespace MeetAndGo.Tests
{

    public class GetEventsQueryTests : IClassFixture<SharedDatabaseFixture>
    {
        public SharedDatabaseFixture Fixture { get; }

        public GetEventsQueryTests(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task ShouldGetEvents()
        {
            await using var context = Fixture.CreateContext();
            var query = new GetEventsQuery
            {
                CityId = 1,
                Day = new DateTime(2030, 12, 17)
            };

            var queryHandler = new GetEventsQueryHandler(AutoMapperMock.GetAutoMapper(), context, new ValidationService());

            var result = await queryHandler.Handle(query);

            result.IsSuccess.Should().BeTrue();
            var queryResult = result.Value;

            queryResult.Events.Should().NotBeNullOrEmpty();
            queryResult.Events.Should().HaveCount(2);
            queryResult.Events[3].Visits.Should().HaveCount(2);
            queryResult.Events[2].Visits.Should().HaveCount(1);
            queryResult.LastVisitId.Should().Be(5);
        }


        [Fact]
        public async Task ShouldFailForWrongDate()
        {
            await using var context = Fixture.CreateContext();
            var queryHandler = new GetEventsQueryHandler(AutoMapperMock.GetAutoMapper(), context, new ValidationService());

            var result = await queryHandler.Handle(new GetEventsQuery
            {
                Day = new DateTime(2010, 12, 16),
                CityId = 5
            });

            result.IsFailure.Should().BeTrue();

            result.Error.Should().Be("Requested events date must be today or future dates.");
        }

        [Fact]
        public async Task ShouldGetEventsForAdvancedSearchFilters()
        {
            await using var context = Fixture.CreateContext();
            var query = new GetEventsQuery
            {
                CityId = 2,
                Day = new DateTime(2030, 12, 17),
                CategoryId = 1,
                TimeOfDay = 1
            };

            var queryHandler = new GetEventsQueryHandler(AutoMapperMock.GetAutoMapper(), context, new ValidationService());

            var result = await queryHandler.Handle(query);

            result.IsSuccess.Should().BeTrue();
            var queryResult = result.Value;

            queryResult.Events.Should().NotBeNullOrEmpty();
            queryResult.Events.Should().HaveCount(1);
            queryResult.Events[1].Visits.Should().HaveCount(1);
            queryResult.LastVisitId.Should().Be(1);
        }

        [Fact]
        public async Task ShouldGetEventsForAdvancedSearchTimeOfDayOnly()
        {
            await using var context = Fixture.CreateContext();
            var query = new GetEventsQuery
            {
                CityId = 1,
                Day = new DateTime(2030, 12, 17),
                TimeOfDay = 3
            };

            var queryHandler = new GetEventsQueryHandler(AutoMapperMock.GetAutoMapper(), context, new ValidationService());

            var result = await queryHandler.Handle(query);

            result.IsSuccess.Should().BeTrue();
            var queryResult = result.Value;

            queryResult.Events.Should().NotBeNullOrEmpty();
            queryResult.Events.Should().HaveCount(1);
            queryResult.Events[3].Visits.Should().HaveCount(1);
            queryResult.LastVisitId.Should().Be(5);
        }

        [Fact]
        public async Task ShouldGetEventsForAdvancedSearchCategoryOnly()
        {
            await using var context = Fixture.CreateContext();
            var query = new GetEventsQuery
            {
                CityId = 1,
                Day = new DateTime(2030, 12, 17),
                CategoryId = 2,
            };

            var queryHandler = new GetEventsQueryHandler(AutoMapperMock.GetAutoMapper(), context, new ValidationService());

            var result = await queryHandler.Handle(query);

            result.IsSuccess.Should().BeTrue();
            var queryResult = result.Value;

            queryResult.Events.Should().NotBeNullOrEmpty();
            queryResult.Events.Should().HaveCount(2);
            queryResult.Events[3].Visits.Should().HaveCount(2);
            queryResult.Events[2].Visits.Should().HaveCount(1);
            queryResult.LastVisitId.Should().Be(5);
        }
    }
}
