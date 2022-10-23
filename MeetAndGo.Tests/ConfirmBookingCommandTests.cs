using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MeetAndGo.Infrastructure.Handlers.Commands.BookingCommands;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Infrastructure.Services.Email;
using MeetAndGo.Tests.Config;
using Moq;
using Xunit;

namespace MeetAndGo.Tests
{
    public class ConfirmBookingCommandTests : IClassFixture<SharedDatabaseFixture>
    {
        public SharedDatabaseFixture Fixture { get; }
        public IMailService MailService { get; }

        public ConfirmBookingCommandTests(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
            MailService = Mock.Of<IMailService>();
        }

        [Fact]
        public async Task ShouldNotConfirmBookingForWrongUser()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("e256b87a-333d-49b7-aab6-2e149313b8cc");

            var command = new ConfirmBookingCommand(1);
            var commandHandler = new ConfirmBookingCommandHandler(context, identityProviderMock.Object, new ValidationService(), MailService);

            var result = await commandHandler.Handle(command);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Booking with id 1 does not belong to the user e256b87a-333d-49b7-aab6-2e149313b8cc");

            context.Bookings.Single(b => b.Id == 1).IsConfirmed.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldConfirmBooking()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("68d1b5ce-8c7e-4f2f-97db-16c9bfe92b57");
            var command = new ConfirmBookingCommand(2);
            var commandHandler = new ConfirmBookingCommandHandler(context, identityProviderMock.Object, new ValidationService(), MailService);

            var result = await commandHandler.Handle(command);
            result.IsSuccess.Should().BeTrue();
            context.Bookings.Single(b => b.Id == 2).IsConfirmed.Should().BeTrue();
        }
    }
}