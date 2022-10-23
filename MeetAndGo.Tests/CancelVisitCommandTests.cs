using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MeetAndGo.Infrastructure.Handlers.Commands.VisitCommands;
using MeetAndGo.Infrastructure.Helpers;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Infrastructure.Services.Email;
using MeetAndGo.Tests.Config;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace MeetAndGo.Tests
{
    public class CancelVisitCommandTests : IClassFixture<SharedDatabaseFixture>
    {
        public SharedDatabaseFixture Fixture { get; }
        public IMailService MailService { get; set; }

        public CancelVisitCommandTests(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
            MailService = Mock.Of<IMailService>();
        }

        [Fact]
        public async Task ShouldCancelVisitWithBooking()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("68d1b5ce-8c7e-4f2f-97db-16c9bfe92b57");

            var command = new CancelVisitCommand(3);
            var commandHandler = new CancelVisitCommandHandler(context, identityProviderMock.Object,
                new ValidationService(), MailService, new DeletedEntitiesService(context), AutoMapperMock.GetAutoMapper());

            var result = await commandHandler.Handle(command);
            result.IsSuccess.Should().BeTrue();

            context.Visits.SingleOrDefault(v => v.Id == 3).Should().BeNull();

            context.Bookings.SingleOrDefault(b => b.Id == 1).Should().BeNull();

            context.DeletedEntities.SingleOrDefault(de => de.IdOrig == 3 && de.Type == DeletedEntityType.Visit)
                .Should().NotBeNull();

            context.DeletedEntities.SingleOrDefault(de => de.IdOrig == 1 && de.Type == DeletedEntityType.Booking)
                .Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldCancelVisitWithoutBooking()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("68d1b5ce-8c7e-4f2f-97db-16c9bfe92b57");

            var command = new CancelVisitCommand(5);
            var commandHandler = new CancelVisitCommandHandler(context, identityProviderMock.Object,
                new ValidationService(), MailService, new DeletedEntitiesService(context), AutoMapperMock.GetAutoMapper());

            var result = await commandHandler.Handle(command);

            result.IsSuccess.Should().BeTrue();

            context.Visits.SingleOrDefault(v => v.Id == 5).Should().BeNull();

            context.DeletedEntities.SingleOrDefault(de => de.IdOrig == 5 && de.Type == DeletedEntityType.Visit)
                .Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldNotCancelVisitForWrongUser()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("e161c8fa-7430-4df2-9040-93704a5b40df");

            var command = new CancelVisitCommand(4);
            var commandHandler = new CancelVisitCommandHandler(context, identityProviderMock.Object,
                new ValidationService(), MailService, new DeletedEntitiesService(context), AutoMapperMock.GetAutoMapper());

            var result = await commandHandler.Handle(command);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Visit: 4 does not belong to e161c8fa-7430-4df2-9040-93704a5b40df");
            context.Visits.SingleOrDefault(v => v.Id == 4).Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldNotCancelIfVisitHasAlreadyStarted()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("68d1b5ce-8c7e-4f2f-97db-16c9bfe92b57");

            var command = new CancelVisitCommand(7);
            var commandHandler = new CancelVisitCommandHandler(context, identityProviderMock.Object,
                new ValidationService(), MailService, new DeletedEntitiesService(context), AutoMapperMock.GetAutoMapper());

            var result = await commandHandler.Handle(command);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Visit can not be cancelled. TOO_LATE");
            var visit = context.Visits.Include(v => v.Bookings).SingleOrDefault(v => v.Id == 7);
            visit.Should().NotBeNull();
            visit.Bookings.Should().NotBeNullOrEmpty();

        }

    }
}