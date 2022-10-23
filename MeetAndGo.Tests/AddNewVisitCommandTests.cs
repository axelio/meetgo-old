using System;
using System.Threading.Tasks;
using FluentAssertions;
using MeetAndGo.Infrastructure.Handlers.Commands.VisitCommands;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Tests.Config;
using Moq;
using Xunit;

namespace MeetAndGo.Tests
{
    public class AddNewVisitCommandTests : IClassFixture<SharedDatabaseFixture>
    {
        public SharedDatabaseFixture Fixture { get; }

        public AddNewVisitCommandTests(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task ShouldAddNewVisit()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("e256b87a-333d-49b7-aab6-2e149313b8cc");

            var command = new AddNewVisitCommand
            {
                Date = new DateTime(2030, 12, 17, 14, 30, 00),
                EventId = 1,
                MaxPersons = 4,
                Price = 35.50m
            };

            var commandHandler = new AddNewVisitCommandHandler(context, identityProviderMock.Object, new ValidationService());
            var result = await commandHandler.Handle(command);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldNotAddNewVisitWhenLimitReached()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("68d1b5ce-8c7e-4f2f-97db-16c9bfe92b57");

            var command = new AddNewVisitCommand
            {
                Date = new DateTime(2030, 12, 17, 14, 30, 00),
                EventId = 3,
                MaxPersons = 4,
                Price = 35.50m
            };

            var commandHandler = new AddNewVisitCommandHandler(context, identityProviderMock.Object, new ValidationService());
            var result = await commandHandler.Handle(command);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("TOO_MANY");
        }

        [Fact]
        public async Task ShouldFailForWrongDate()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("e256b87a-333d-49b7-aab6-2e149313b8cc");

            var command = new AddNewVisitCommand
            {
                Date = new DateTime(2020, 12, 17, 14, 30, 00),
                EventId = 1,
                MaxPersons = 4,
                Price = 35.50m
            };

            var commandHandler = new AddNewVisitCommandHandler(context, identityProviderMock.Object, new ValidationService());
            var result = await commandHandler.Handle(command);

            result.IsSuccess.Should().BeFalse();
        }
    }
}