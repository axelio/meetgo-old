using System.Collections.Generic;
using System.Threading.Tasks;
using MeetAndGo.Data.Dto;
using MeetAndGo.Infrastructure.Handlers.Commands.BookingCommands;
using MeetAndGo.Infrastructure.Handlers.Queries.BookingQueries;
using MeetAndGo.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetAndGo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BookingController : ControllerBase
    {
        private readonly IResultFailureLogger _failureLogger;
        private readonly IQueryHandler<GetClientApproachingBookingsQuery, List<ClientBookingDto>> _getClientApproachingBookingsQueryHandler;
        private readonly ICommandHandler<MakeBookingCommand> _makeBookingCommandHandler;
        private readonly ICommandHandler<CancelBookingCommand> _cancelBookingCommandHandler;
        private readonly ICommandHandler<ConfirmBookingCommand> _confirmBookingCommandHandler;

        public BookingController(
            IResultFailureLogger failureLogger,
            IQueryHandler<GetClientApproachingBookingsQuery, List<ClientBookingDto>> getClientApproachingBookingsQueryHandler,
            ICommandHandler<MakeBookingCommand> makeBookingCommandHandler,
            ICommandHandler<CancelBookingCommand> cancelBookingCommandHandler,
            ICommandHandler<ConfirmBookingCommand> confirmBookingCommandHandler)
        {
            _failureLogger = failureLogger;
            _getClientApproachingBookingsQueryHandler = getClientApproachingBookingsQueryHandler;
            _makeBookingCommandHandler = makeBookingCommandHandler;
            _cancelBookingCommandHandler = cancelBookingCommandHandler;
            _confirmBookingCommandHandler = confirmBookingCommandHandler;
        }

        /// <summary>
        /// Get all approaching bookings for client
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("client")]
        [Authorize(Policy = "Client")]
        public async Task<IActionResult> GetClientApproachingBookings()
        {
            var result = await _getClientApproachingBookingsQueryHandler.Handle(new GetClientApproachingBookingsQuery());
            return Ok(result);
        }


        /// <summary>
        /// Make booking for client
        /// </summary>
        [HttpPost]
        [Route("make/{visitId}")]
        [Authorize(Policy = "Client")]
        public async Task<IActionResult> MakeBooking(int visitId)
        {
            var command = new MakeBookingCommand(visitId);
            var result = await _makeBookingCommandHandler.Handle(command);
            if (result.IsFailure)
            {
                _failureLogger.LogFailureError(result, command);
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// cancel booking by customer
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("cancel/{bookingId}")]
        [Authorize(Policy = "Client")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var command = new CancelBookingCommand(bookingId);
            var result = await _cancelBookingCommandHandler.Handle(command);

            if (result.IsFailure)
            {
                _failureLogger.LogFailureError(result, command);
                return BadRequest(result.Error);
            }
            return NoContent();
        }

        /// <summary>
        /// confirm booking by company
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("confirm/{bookingId}")]
        [Authorize(Policy = "Company")]
        public async Task<IActionResult> ConfirmBooking(int bookingId)
        {
            var command = new ConfirmBookingCommand(bookingId);
            var result = await _confirmBookingCommandHandler.Handle(command);

            if (result.IsFailure)
            {
                _failureLogger.LogFailureError(result, command);
                return BadRequest(result.Error);
            }
            return NoContent();
        }
    }
}