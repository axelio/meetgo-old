using System.Threading.Tasks;
using FluentAssertions;
using MeetAndGo.Infrastructure.Handlers.Queries.BookingQueries;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Tests.Config;
using Moq;
using Xunit;

namespace MeetAndGo.Tests
{
    public class GetClientApproachingBookingsQueryTests : IClassFixture<SharedDatabaseFixture>
    {
        public SharedDatabaseFixture Fixture { get; }

        public GetClientApproachingBookingsQueryTests(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task ShouldGetApproachingBookings()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("a52dbf80-2e31-4817-b701-2a4c96c2b8f4");

            var queryHandler = new GetClientApproachingBookingsQueryHandler(AutoMapperMock.GetAutoMapper(), context, identityProviderMock.Object);
            var result = await queryHandler.Handle(new GetClientApproachingBookingsQuery());

            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task ShouldNotGetApproachingBookings()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("cd11d467-0ff5-4c73-83f7-646ea62b803b");

            var queryHandler = new GetClientApproachingBookingsQueryHandler(AutoMapperMock.GetAutoMapper(), context, identityProviderMock.Object);
            var result = await queryHandler.Handle(new GetClientApproachingBookingsQuery());

            result.Count.Should().Be(0);
        }
    }
}