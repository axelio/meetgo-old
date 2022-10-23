using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MeetAndGo.Infrastructure.Handlers.Queries.BookingQueries;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Tests.Config;
using Moq;
using Xunit;

namespace MeetAndGo.Tests
{
    public class GetCompanyApproachingVisitsQueryTests : IClassFixture<SharedDatabaseFixture>
    {
        public SharedDatabaseFixture Fixture { get; }

        public GetCompanyApproachingVisitsQueryTests(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task ShouldGetApproachingVisits()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("68d1b5ce-8c7e-4f2f-97db-16c9bfe92b57"); //gdansk company

            var queryHandler = new GetCompanyApproachingVisitsQueryHandler(AutoMapperMock.GetAutoMapper(), identityProviderMock.Object, context);
            var result = await queryHandler.Handle(new GetCompanyApproachingVisitsQuery());

            result.Count.Should().Be(5);
            result.First(v => v.Id == 3).Bookings.Should().NotBeNullOrEmpty();
            result.First(v => v.Id == 4).Bookings.Should().BeNullOrEmpty();
        }
    }
}