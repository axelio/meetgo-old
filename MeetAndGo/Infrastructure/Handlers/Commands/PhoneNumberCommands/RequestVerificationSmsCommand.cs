using System;
using System.Threading.Tasks;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Services.Sms;
using MeetAndGo.Infrastructure.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace MeetAndGo.Infrastructure.Handlers.Commands.PhoneNumberCommands
{
    public class RequestVerificationSmsCommand : ICommand
    {
        public class RequestVerificationSmsCommandHandler : ICommandHandler<RequestVerificationSmsCommand>
        {
            private readonly IIdentityProvider _identityProvider;
            private readonly UserManager<User> _userManager;
            private readonly ISmsService _smsService;
            private readonly IMemoryCache _cache;
            private readonly ILogger<RequestVerificationSmsCommandHandler> _logger;

            public RequestVerificationSmsCommandHandler(
                IIdentityProvider identityProvider,
                UserManager<User> userManager,
                ISmsService smsService,
                IMemoryCache cache,
                ILogger<RequestVerificationSmsCommandHandler> logger)
            {
                _identityProvider = identityProvider;
                _userManager = userManager;
                _smsService = smsService;
                _cache = cache;
                _logger = logger;
            }

            public async Task<Result> Handle(RequestVerificationSmsCommand command)
            {
                var cacheCheckResult = PerformCacheCheck();
                if (cacheCheckResult.IsFailure)
                    return Result.Ok(); // don't let client know about this protection

                var customerId = _identityProvider.GetUserIdFromClaims();
                var customer = await _userManager.FindByIdAsync(customerId);

                if (string.IsNullOrEmpty(customer.PhoneNumber))
                    return Result.Fail("Customer has no phone number.");
                if (customer.PhoneNumberConfirmed)
                    return Result.Fail("Customer number is already confirmed");

                var token = await _userManager.GenerateChangePhoneNumberTokenAsync(customer, customer.PhoneNumber);
                var smsResult = await SendSms(customer, token);

                return smsResult.IsFailure ? Result.Fail(smsResult.Error) : Result.Ok();
            }

            private async Task<Result> SendSms(User customer, string token) =>
                await _smsService.SendSmsAsync(new SmsRequest
                {
                    Message = $"Kod aktywacyjny  do meetgo to: {token}",
                    PhoneNumber = customer.PhoneNumber
                });

            private Result PerformCacheCheck()
            {
                var userMail = _identityProvider.GetUserMailFromClaims();
                var valueExists = _cache.TryGetValue(userMail, out int requests);
                var twoRequests = 2;

                if (!valueExists)
                {
                    _cache.Set(userMail, 1, TimeSpan.FromMinutes(30));
                    return Result.Ok();
                }

                if (requests < twoRequests)
                {
                    _cache.Set(userMail, requests + 1, TimeSpan.FromMinutes(30));
                    return Result.Ok();
                }

                _logger.LogWarning($"Too many sms verification requests from user: {userMail}");
                return Result.Fail("TOO_MANY_SMS");
            }
        }
    }
}