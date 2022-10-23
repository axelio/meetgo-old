using System;
using System.Threading.Tasks;
using MeetAndGo.Infrastructure.Handlers.Commands.PhoneNumberCommands;
using MeetAndGo.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetAndGo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PhoneNumberController : ControllerBase
    {
        private readonly IResultFailureLogger _failureLogger;
        private readonly ICommandHandler<AddPhoneNumberCommand> _addPhoneNumberCommandHandler;
        private readonly ICommandHandler<VerifyPhoneNumberCommand> _verifyPhoneNumberCommandHandler;
        private readonly ICommandHandler<RequestVerificationSmsCommand> _requestVerificationSmsCommandHandler;

        public PhoneNumberController(IResultFailureLogger failureLogger,
            ICommandHandler<AddPhoneNumberCommand> addPhoneNumberCommandHandler,
            ICommandHandler<VerifyPhoneNumberCommand> verifyPhoneNumberCommandHandler,
            ICommandHandler<RequestVerificationSmsCommand> requestVerificationSmsCommandHandler)
        {
            _failureLogger = failureLogger;
            _addPhoneNumberCommandHandler = addPhoneNumberCommandHandler;
            _verifyPhoneNumberCommandHandler = verifyPhoneNumberCommandHandler;
            _requestVerificationSmsCommandHandler = requestVerificationSmsCommandHandler;
        }

        /// <summary>
        /// Adds phone number
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        [Authorize(Policy = "client")]
        public async Task<IActionResult> AddPhoneNumber([FromBody] AddPhoneNumberCommand command)
        {
            var result = await _addPhoneNumberCommandHandler.Handle(command);
            if (result.IsFailure)
            {
                _failureLogger.LogFailureError(result, command);
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Verifies if the token is correct
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("verify")]
        [Authorize(Policy = "client")]
        public async Task<IActionResult> VerifyPhoneNumber([FromBody] VerifyPhoneNumberCommand command)
        {
            var result = await _verifyPhoneNumberCommandHandler.Handle(command);
            if (result.IsFailure)
            {
                _failureLogger.LogFailureError(result, command);
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// request sending verification token
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("request-token")]
        [Authorize(Policy = "client")]
        public async Task<IActionResult> RequestVerificationToken()
        {
            var command = new RequestVerificationSmsCommand();
            var result = await _requestVerificationSmsCommandHandler.Handle(command);
            if (result.IsFailure)
            {
                _failureLogger.LogFailureError(result, command);
                return BadRequest(result.Error);
            }

            return NoContent();
        }
    }
}