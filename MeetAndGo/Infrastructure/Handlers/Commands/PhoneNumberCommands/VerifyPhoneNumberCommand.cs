using System.Threading.Tasks;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Services;
using MeetAndGo.Infrastructure.Utils;
using MeetAndGo.Infrastructure.Validators;
using Microsoft.AspNetCore.Identity;

namespace MeetAndGo.Infrastructure.Handlers.Commands.PhoneNumberCommands
{
    public class VerifyPhoneNumberCommand : ICommand
    {
        public string Token { get; set; }
    }

    public class VerifyPhoneNumberCommandHandler : ICommandHandler<VerifyPhoneNumberCommand>
    {
        private readonly IValidationService _validationService;
        private readonly UserManager<User> _userManager;
        private readonly IIdentityProvider _identityProvider;
        private const string WrongTokenError = "WRONG_SMS_TOKEN";

        public VerifyPhoneNumberCommandHandler(
            IValidationService validationService,
            UserManager<User> userManager,
            IIdentityProvider identityProvider)
        {
            _validationService = validationService;
            _userManager = userManager;
            _identityProvider = identityProvider;
        }

        public async Task<Result> Handle(VerifyPhoneNumberCommand command)
        {
            var validation = _validationService.ValidateRules(new VerifyPhoneNumberCommandValidator(), command);
            if (validation.IsFailure) return Result.Fail(validation.Error);

            var customerUserId = _identityProvider.GetUserIdFromClaims();
            var customer = await _userManager.FindByIdAsync(customerUserId);

            var confirmResult = await _userManager.ChangePhoneNumberAsync(customer, customer.PhoneNumber, command.Token);
            return confirmResult.Succeeded ? Result.Ok() : Result.Fail(WrongTokenError);
        }
    }
}