using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Handlers.Commands.BookingCommands;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Infrastructure.Services.Email;
using MeetAndGo.Tests.Config;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace MeetAndGo.Tests
{
    public class MakeBookingCommandTests : IClassFixture<SharedDatabaseFixture>
    {
        public SharedDatabaseFixture Fixture { get; }

        public IMailService MailService { get; set; }

        public MakeBookingCommandTests(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;

            MailService = Mock.Of<IMailService>();
        }

        [Fact]
        public async Task ShouldMakeBooking()
        {
            //arrange
            await using var context = Fixture.CreateContext();

            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("cd11d467-0ff5-4c73-83f7-646ea62b803b");

            var visit = context.Visits.First(v => v.Id == 1);

            //act
            var commandHandler = new MakeBookingCommandCommandHandler(context, identityProviderMock.Object, new ValidationService(), MailService);
            var result = await commandHandler.Handle(new MakeBookingCommand(visit.Id));

            //assert
            result.IsSuccess.Should().BeTrue();
            visit.Bookings.Should().NotBeNullOrEmpty();
            visit.Bookings.First().UserId.Should().Be("cd11d467-0ff5-4c73-83f7-646ea62b803b");
            visit.Bookings.First().IsConfirmed.Should().BeFalse();
            visit.IsBooked.Should().BeTrue();
            visit.BookingsNumber.Should().Be(1);
        }

        [Fact]
        public async Task ShouldMakeEnrollmentBooking()
        {
            //arrange
            await using var context = Fixture.CreateContext();

            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("cd11d467-0ff5-4c73-83f7-646ea62b803b");

            var visit = context.Visits.First(v => v.Id == 8);

            //act
            var commandHandler = new MakeBookingCommandCommandHandler(context, identityProviderMock.Object, new ValidationService(), MailService);
            var result = await commandHandler.Handle(new MakeBookingCommand(visit.Id));

            //assert
            result.IsSuccess.Should().BeTrue();
            visit.Bookings.Should().NotBeNullOrEmpty();
            visit.Bookings.Last().UserId.Should().Be("cd11d467-0ff5-4c73-83f7-646ea62b803b");
            visit.Bookings.Last().IsConfirmed.Should().BeFalse();
            visit.IsBooked.Should().BeFalse();
            visit.BookingsNumber.Should().Be(2);
        }

        [Fact]
        public async Task ShouldNotMakeBookingWhenAlreadyBooked()
        {
            //arrange
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("cd11d467-0ff5-4c73-83f7-646ea62b803b");
            var visit = context.Visits.Include(v => v.Bookings).First(v => v.Id == 6);

            //act
            var commandHandler = new MakeBookingCommandCommandHandler(context, identityProviderMock.Object, new ValidationService(), MailService);
            var result = await commandHandler.Handle(new MakeBookingCommand(visit.Id));

            //assert
            result.IsFailure.Should().BeTrue();
            visit.Bookings.Should().NotBeNullOrEmpty();
            visit.Bookings.First().UserId.Should().Be("a52dbf80-2e31-4817-b701-2a4c96c2b8f4");
            result.Error.Should().Be("ALREADY_BOOKED");
        }

        [Fact]
        public async Task ShouldNotMakeBookingWhenAlreadyHasTwoBookings()
        {
            //arrange
            await using var context = Fixture.CreateContext();

            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("a52dbf80-2e31-4817-b701-2a4c96c2b8f4");

            var user = context.Users.Single(x => x.Id == "a52dbf80-2e31-4817-b701-2a4c96c2b8f4");
            var visit = context.Visits.Single(x => x.Id == 5);
            context.Bookings.Add(new Booking
            {
                Customer = user,
                Visit = visit
            });
            context.SaveChanges();

            //act
            var commandHandler = new MakeBookingCommandCommandHandler(context, identityProviderMock.Object, new ValidationService(), MailService);
            var result = await commandHandler.Handle(new MakeBookingCommand(4));

            //assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("TOO_MANY_BOOKINGS");
            context.Visits.Single(x => x.Id == 4).Bookings.Should().BeNullOrEmpty();
            context.Visits.Single(x => x.Id == 4).IsBooked.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ShouldFailAndReturnNoPhoneNumber(string number)
        {
            //arrange
            await using var context = Fixture.CreateContext();

            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("a52dbf80-2e31-4817-b701-2a4c96c2b8f4");

            var user = context.Users.Single(x => x.Id == "a52dbf80-2e31-4817-b701-2a4c96c2b8f4");
            user.PhoneNumber = number;
            user.PhoneNumberConfirmed = false;
            context.SaveChanges();

            //act
            var commandHandler = new MakeBookingCommandCommandHandler(context, identityProviderMock.Object, new ValidationService(), MailService);
            var result = await commandHandler.Handle(new MakeBookingCommand(4));

            //assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("NO_PHONE_NUMBER");
            context.Visits.Single(x => x.Id == 4).Bookings.Should().BeNullOrEmpty();
            context.Visits.Single(x => x.Id == 4).IsBooked.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldFailAndReturnNotVerifiedPhoneNumber()
        {
            //arrange
            await using var context = Fixture.CreateContext();

            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("a52dbf80-2e31-4817-b701-2a4c96c2b8f4");

            var user = context.Users.Single(x => x.Id == "a52dbf80-2e31-4817-b701-2a4c96c2b8f4");
            user.PhoneNumber = "+48111222333";
            user.PhoneNumberConfirmed = false;
            context.SaveChanges();

            //act
            var commandHandler = new MakeBookingCommandCommandHandler(context, identityProviderMock.Object, new ValidationService(), MailService);
            var result = await commandHandler.Handle(new MakeBookingCommand(4));

            //assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("PHONE_NUMBER_NOT_VERIFIED");
            context.Visits.Single(x => x.Id == 4).Bookings.Should().BeNullOrEmpty();
            context.Visits.Single(x => x.Id == 4).IsBooked.Should().BeFalse();
        }
    }
}