using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeetAndGo.Data.Dto;
using MeetAndGo.Infrastructure.Handlers.Commands.VisitCommands;
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
    public class VisitController : ControllerBase
    {
        private readonly IQueryHandler<GetCompanyApproachingVisitsQuery, List<CompanyVisitDisplayDto>> _getCompanyApproachingBookingsQueryHandler;
        private readonly ICommandHandler<CancelVisitCommand> _cancelVisitCommandHandler;
        private readonly ICommandHandler<AddNewVisitCommand> _addNewVisitsCommandHandler;
        private readonly IResultFailureLogger _failureLogger;

        public VisitController(
            IQueryHandler<GetCompanyApproachingVisitsQuery, List<CompanyVisitDisplayDto>> getCompanyApproachingBookingsQueryHandler,
            ICommandHandler<CancelVisitCommand> cancelVisitCommandHandler,
            ICommandHandler<AddNewVisitCommand> addNewVisitsCommandHandler,
            IResultFailureLogger failureLogger)
        {
            _getCompanyApproachingBookingsQueryHandler = getCompanyApproachingBookingsQueryHandler;
            _cancelVisitCommandHandler = cancelVisitCommandHandler;
            _addNewVisitsCommandHandler = addNewVisitsCommandHandler;
            _failureLogger = failureLogger;
        }

        /// <summary>
        /// Get all approaching visits with optional bookings for company
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("company")]
        [Authorize(Policy = "Company")]
        public async Task<IActionResult> GetCompanyApproachingBookings()
        {
            return Ok(await _getCompanyApproachingBookingsQueryHandler.Handle(new GetCompanyApproachingVisitsQuery()));
        }

        /// <summary>
        /// Cancel visit and booking if exists
        /// </summary>
        /// <param name="visitId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("cancel/{visitId}")]
        [Authorize(Policy = "Company")]
        public async Task<IActionResult> CancelVisit(int visitId)
        {
            var command = new CancelVisitCommand(visitId);
            var result = await _cancelVisitCommandHandler.Handle(command);

            if (result.IsFailure)
            {
                _failureLogger.LogFailureError(result, command);
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Add new visits for company to existing events.
        /// </summary>
        /// <param name="cmd">command body</param>
        /// <returns></returns>
        [HttpPost]
        [Route("add-new")]
        [Authorize(Policy = "Company")]
        public async Task<IActionResult> AddVisit([FromBody] AddNewVisitCommand cmd)
        {
            var result = await _addNewVisitsCommandHandler.Handle(cmd);

            if (result.IsFailure)
            {
                _failureLogger.LogFailureError(result, cmd);
                return BadRequest(result.Error);
            }

            return Ok();
        }
    }
}