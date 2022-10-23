using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Handlers.Commands.PhoneNumberCommands;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Tests.Config;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace MeetAndGo.Tests
{

    public class VerifyPhoneNumberCommandTests : IClassFixture<SharedDatabaseFixture>
    {
        public SharedDatabaseFixture Fixture { get; }

        public VerifyPhoneNumberCommandTests(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task ShouldVerifyPhoneNumber()
        {
            await using var context = Fixture.CreateContext();
            var userId = "a52dbf80-2e31-4817-b701-2a4c96c2b8f4";

            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns(userId);

            var user = context.Users.Single(x => x.Id == userId);
            user.PhoneNumberConfirmed = false;
            context.SaveChanges();

            var userManagerMock = GetUserManagerMock();
            userManagerMock.Setup(s => s.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManagerMock.Setup(s => s.ChangePhoneNumberAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var command = new VerifyPhoneNumberCommand
            {
                Token = "token"
            };

            var handler = new VerifyPhoneNumberCommandHandler(new ValidationService(), userManagerMock.Object, identityProviderMock.Object);

            var result = await handler.Handle(command);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldFailForWrongToken()
        {
            await using var context = Fixture.CreateContext();
            var userId = "a52dbf80-2e31-4817-b701-2a4c96c2b8f4";

            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns(userId);

            var user = context.Users.Single(x => x.Id == userId);
            user.PhoneNumberConfirmed = false;
            context.SaveChanges();

            var userManagerMock = GetUserManagerMock();
            userManagerMock.Setup(s => s.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManagerMock.Setup(s => s.ChangePhoneNumberAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            var command = new VerifyPhoneNumberCommand
            {
                Token = "not_valid_token"
            };

            var handler = new VerifyPhoneNumberCommandHandler(new ValidationService(), userManagerMock.Object, identityProviderMock.Object);

            var result = await handler.Handle(command);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo("WRONG_SMS_TOKEN");
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