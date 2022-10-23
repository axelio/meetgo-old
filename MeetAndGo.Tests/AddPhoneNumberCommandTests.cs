using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Handlers.Commands.PhoneNumberCommands;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Tests.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MeetAndGo.Tests
{
    public class AddPhoneNumberCommandTests : IClassFixture<SharedDatabaseFixture>
    {
        public SharedDatabaseFixture Fixture { get; }

        public AddPhoneNumberCommandTests(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task ShouldFailWhenUserManagerFailed()
        {
            await using var context = Fixture.CreateContext();
            var userId = "a52dbf80-2e31-4817-b701-2a4c96c2b8f4";
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns(userId);

            var loggerMock = new Mock<ILogger<AddPhoneNumberCommandHandler>>();

            var user = context.Users.Single(x => x.Id == "a52dbf80-2e31-4817-b701-2a4c96c2b8f4");
            user.PhoneNumber = null;
            user.PhoneNumberConfirmed = false;
            context.SaveChanges();

            var userManagerMock = GetUserManagerMock();
            userManagerMock.Setup(s => s.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManagerMock.Setup(s => s.SetPhoneNumberAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            var command = new AddPhoneNumberCommand
            {
                PhoneNumber = "796000111"
            };

            var handler = new AddPhoneNumberCommandHandler(new ValidationService(), identityProviderMock.Object, userManagerMock.Object, loggerMock.Object);

            var result = await handler.Handle(command);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo("Unexpected error while setting a phone number");
        }

        [Fact]
        public async Task ShouldFailWhenPhoneNumberAlreadyAdded()
        {
            await using var context = Fixture.CreateContext();
            var userId = "a52dbf80-2e31-4817-b701-2a4c96c2b8f4";
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns(userId);

            var loggerMock = new Mock<ILogger<AddPhoneNumberCommandHandler>>();

            var user = context.Users.Single(x => x.Id == "a52dbf80-2e31-4817-b701-2a4c96c2b8f4");

            var userManagerMock = GetUserManagerMock();
            userManagerMock.Setup(s => s.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManagerMock.Setup(s => s.SetPhoneNumberAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var command = new AddPhoneNumberCommand
            {
                PhoneNumber = "796000111"
            };

            var handler = new AddPhoneNumberCommandHandler(new ValidationService(), identityProviderMock.Object, userManagerMock.Object, loggerMock.Object);

            var result = await handler.Handle(command);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo($"Unexpected request: customer {userId} already has a phone number");
        }

        [Fact]
        public async Task ShouldAddPhoneNumber()
        {
            await using var context = Fixture.CreateContext();
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns("a52dbf80-2e31-4817-b701-2a4c96c2b8f4");

            var loggerMock = new Mock<ILogger<AddPhoneNumberCommandHandler>>();

            var user = context.Users.Single(x => x.Id == "a52dbf80-2e31-4817-b701-2a4c96c2b8f4");
            user.PhoneNumber = null;
            user.PhoneNumberConfirmed = false;
            context.SaveChanges();

            var userManagerMock = GetUserManagerMock();
            userManagerMock.Setup(s => s.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManagerMock.Setup(s => s.SetPhoneNumberAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var command = new AddPhoneNumberCommand
            {
                PhoneNumber = "796000111"
            };

            var handler = new AddPhoneNumberCommandHandler(new ValidationService(), identityProviderMock.Object, userManagerMock.Object, loggerMock.Object);

            var result = await handler.Handle(command);

            result.IsSuccess.Should().BeTrue();
        }

        private static Mock<UserManager<User>> GetUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();
            var mgr = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<User>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<User>());
            return mgr;
        }
    }
}