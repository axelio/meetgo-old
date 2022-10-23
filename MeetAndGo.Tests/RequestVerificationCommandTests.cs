using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Handlers.Commands.PhoneNumberCommands;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Services.Sms;
using MeetAndGo.Infrastructure.Utils;
using MeetAndGo.Tests.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static MeetAndGo.Infrastructure.Handlers.Commands.PhoneNumberCommands.RequestVerificationSmsCommand;

namespace MeetAndGo.Tests
{
    public class RequestVerificationCommandTests : IClassFixture<SharedDatabaseFixture>
    {
        public SharedDatabaseFixture Fixture { get; }

        public RequestVerificationCommandTests(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task ShouldReturnResulOkWhenSmsRequestsExceeded()
        {
            await using var context = Fixture.CreateContext();
            var userId = "a52dbf80-2e31-4817-b701-2a4c96c2b8f4";
            var userMail = "first@customer.pl";

            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns(userId);
            identityProviderMock.Setup(provider => provider.GetUserMailFromClaims()).Returns(userMail);

            var user = context.Users.Single(x => x.Id == userId);
            context.SaveChanges();
            var userManagerMock = GetUserManagerMock(user);

            var smsServiceMock = new Mock<ISmsService>();
            smsServiceMock.Setup(s => s.SendSmsAsync(It.IsAny<SmsRequest>())).ReturnsAsync(Result.Ok);
            var loggerMock = new Mock<ILogger<RequestVerificationSmsCommandHandler>>();

            var memoryCache = GetMemoryCache();
            memoryCache.Set(userMail, 2);

            var command = new RequestVerificationSmsCommand();

            var handler = new RequestVerificationSmsCommandHandler(
                identityProviderMock.Object,
                userManagerMock.Object,
                smsServiceMock.Object,
                memoryCache,
                loggerMock.Object);

            var result = await handler.Handle(command);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldNotSendIfNumberConfirmed()
        {
            await using var context = Fixture.CreateContext();
            var userId = "a52dbf80-2e31-4817-b701-2a4c96c2b8f4";
            var userMail = "first@customer.pl";

            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns(userId);
            identityProviderMock.Setup(provider => provider.GetUserMailFromClaims()).Returns(userMail);

            var user = context.Users.Single(x => x.Id == userId);
            context.SaveChanges();

            var userManagerMock = GetUserManagerMock(user);

            var smsServiceMock = new Mock<ISmsService>();
            smsServiceMock.Setup(s => s.SendSmsAsync(It.IsAny<SmsRequest>())).ReturnsAsync(Result.Ok);
            var loggerMock = new Mock<ILogger<RequestVerificationSmsCommandHandler>>();

            var memoryCache = GetMemoryCache();

            var command = new RequestVerificationSmsCommand();

            var handler = new RequestVerificationSmsCommandHandler(
                identityProviderMock.Object,
                userManagerMock.Object,
                smsServiceMock.Object,
                memoryCache,
                loggerMock.Object);

            var result = await handler.Handle(command);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo("Customer number is already confirmed");
        }

        [Fact]
        public async Task ShouldNotSendWhenNoPhoneNumber()
        {
            await using var context = Fixture.CreateContext();
            var userId = "a52dbf80-2e31-4817-b701-2a4c96c2b8f4";
            var userMail = "first@customer.pl";

            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns(userId);
            identityProviderMock.Setup(provider => provider.GetUserMailFromClaims()).Returns(userMail);

            var user = context.Users.Single(x => x.Id == userId);
            user.PhoneNumber = null;
            context.SaveChanges();

            var userManagerMock = GetUserManagerMock(user);

            var smsServiceMock = new Mock<ISmsService>();
            smsServiceMock.Setup(s => s.SendSmsAsync(It.IsAny<SmsRequest>())).ReturnsAsync(Result.Ok);
            var loggerMock = new Mock<ILogger<RequestVerificationSmsCommandHandler>>();

            var memoryCache = GetMemoryCache();

            var command = new RequestVerificationSmsCommand();

            var handler = new RequestVerificationSmsCommandHandler(
                identityProviderMock.Object,
                userManagerMock.Object,
                smsServiceMock.Object,
                memoryCache,
                loggerMock.Object);

            var result = await handler.Handle(command);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo("Customer has no phone number.");
        }

        [Fact]
        public async Task ShouldSendVerificationSms()
        {
            await using var context = Fixture.CreateContext();
            var userId = "cd11d467-0ff5-4c73-83f7-646ea62b803b";
            var userMail = "second@customer.pl";

            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(provider => provider.GetUserIdFromClaims()).Returns(userId);
            identityProviderMock.Setup(provider => provider.GetUserMailFromClaims()).Returns(userMail);

            var user = context.Users.Single(x => x.Id == userId);
            user.PhoneNumberConfirmed = false;
            context.SaveChanges();

            var userManagerMock = GetUserManagerMock(user);

            var smsServiceMock = new Mock<ISmsService>();
            smsServiceMock.Setup(s => s.SendSmsAsync(It.IsAny<SmsRequest>())).ReturnsAsync(Result.Ok);
            var loggerMock = new Mock<ILogger<RequestVerificationSmsCommandHandler>>();

            var memoryCache = GetMemoryCache();

            var command = new RequestVerificationSmsCommand();

            var handler = new RequestVerificationSmsCommandHandler(
                identityProviderMock.Object,
                userManagerMock.Object,
                smsServiceMock.Object,
                memoryCache,
                loggerMock.Object);

            var result = await handler.Handle(command);

            result.IsSuccess.Should().BeTrue();
        }

        private static Mock<UserManager<User>> GetUserManagerMock(User user)
        {
            var store = new Mock<IUserStore<User>>();
            var mgr = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<User>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<User>());

            mgr.Setup(s => s.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            mgr.Setup(s => s.GenerateChangePhoneNumberTokenAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync("CODE");
            return mgr;
        }

        private static IMemoryCache GetMemoryCache()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();
            return memoryCache;
        }
    }
}