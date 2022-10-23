using System.Threading.Tasks;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Infrastructure.Utils;
using MeetAndGo.Infrastructure.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MeetAndGo.Infrastructure.Handlers.Commands.PhoneNumberCommands
{
    public class AddPhoneNumberCommand : ICommand
    {
        public string PhoneNumber { get; set; }
    }

    public sealed class AddPhoneNumberCommandHandler : ICommandHandler<AddPhoneNumberCommand>
    {
        private readonly IValidationService _validationService;
        private readonly IIdentityProvider _identityProvider;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AddPhoneNumberCommandHandler> _logger;

        public AddPhoneNumberCommandHandler(
            IValidationService validationService,
            IIdentityProvider identityProvider,
            UserManager<User> userManager,
            ILogger<AddPhoneNumberCommandHandler> logger)
        {
            _validationService = validationService;
            _identityProvider = identityProvider;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result> Handle(AddPhoneNumberCommand command)
        {
            var validation = _validationService.ValidateRules(new AddPhoneNumberCommandValidator(), command);
            if (validation.IsFailure) return Result.Fail(validation.Error);

            var customerUserId = _identityProvider.GetUserIdFromClaims();
            var customer = await _userManager.FindByIdAsync(customerUserId);

            if (!string.IsNullOrEmpty(customer.PhoneNumber))
                return Result.Fail($"Unexpected request: customer {customerUserId} already has a phone number");

            var setPhoneResult = await _userManager.SetPhoneNumberAsync(customer, PhoneNumberFactory.CreatePolishNumber(command.PhoneNumber));
            if (!setPhoneResult.Succeeded)
            {
                _logger.LogError($"APP_ERROR: Setting phone number for {customerUserId} error: {JsonConvert.SerializeObject(setPhoneResult.Errors)}");
                return Result.Fail("Unexpected error while setting a phone number");
            }

            return Result.Ok();
        }
    }
}