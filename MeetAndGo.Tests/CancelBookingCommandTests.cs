using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MeetAndGo.Infrastructure.Handlers.Commands.BookingCommands;
using MeetAndGo.Infrastructure.Helpers;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Infrastructure.Services.Email;
using MeetAndGo.Tests.Config;
using Moq;
using Xunit;

namespace MeetAndGo.Tests
{
    public class CancelBookingCommandTests : IClassFixture<SharedDatabaseFixture>
    {
        public SharedDatabaseFixture Fixture { get; }
        public IMailService MailService { get; set; }

        public CancelBookingCommandTests(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
            MailService = Mock.Of<IMailService>();
        }

        [Fact]
        public async Task ShouldCancelBooking()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("a52dbf80-2e31-4817-b701-2a4c96c2b8f4");

            var booking = context.Bookings.First(b => b.Id == 1);

            var visit = context.Visits.First(v => v.Id == booking.Id);

            var commandHandler = new CancelBookingCommandHandler(context,
                identityProviderMock.Object, new ValidationService(), MailService, new DeletedEntitiesService(context), AutoMapperMock.GetAutoMapper());

            var result = await commandHandler.Handle(new CancelBookingCommand(booking.Id));

            result.IsSuccess.Should().BeTrue();

            visit.Bookings.Should().BeNullOrEmpty();
            visit.IsBooked.Should().BeFalse();
            visit.BookingsNumber.Should().Be(0);

            context.DeletedEntities.SingleOrDefault(de => de.IdOrig == 1 && de.Type == DeletedEntityType.Booking)
                .Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldCancelEnrollmentBooking()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("third-customer");

            var booking = context.Bookings.First(b => b.Id == 4);

            var visit = context.Visits.First(v => v.Id == booking.Id);

            var commandHandler = new CancelBookingCommandHandler(context,
                identityProviderMock.Object, new ValidationService(), MailService, new DeletedEntitiesService(context), AutoMapperMock.GetAutoMapper());

            var result = await commandHandler.Handle(new CancelBookingCommand(booking.Id));

            result.IsSuccess.Should().BeTrue();

            visit.Bookings.Should().BeNullOrEmpty();
            visit.IsBooked.Should().BeFalse();
            visit.BookingsNumber.Should().Be(0);

            context.DeletedEntities.SingleOrDefault(de => de.IdOrig == 4 && de.Type == DeletedEntityType.Booking)
                .Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldNotCancelBookingWhenWrongUser()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("cd11d467-0ff5-4c73-83f7-646ea62b803b");
            var booking = context.Bookings.First(b => b.Id == 1);

            var commandHandler = new CancelBookingCommandHandler(context,
                identityProviderMock.Object, new ValidationService(), MailService, new DeletedEntitiesService(context), AutoMapperMock.GetAutoMapper());

            var result = await commandHandler.Handle(new CancelBookingCommand(booking.Id));

            result.IsFailure.Should().BeTrue();

            result.Error.Should().Be($"Booking: {booking.Id} does not belong to cd11d467-0ff5-4c73-83f7-646ea62b803b");
        }
    }
}