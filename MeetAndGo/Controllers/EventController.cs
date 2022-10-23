using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetAndGo.Data.Dto;
using MeetAndGo.Infrastructure.Handlers.Queries.EventQueries;
using MeetAndGo.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetAndGo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EventController : ControllerBase
    {
        private readonly IResultFailureLogger _resultFailureLogger;
        private readonly IQueryHandler<GetEventsQuery, Result<GetEventsQueryResult>> _getEventsQueryHandler;
        private readonly IQueryHandler<GetEventsNamesQuery, List<EventNameDto>> _getEventNamesQueryHandler;

        public EventController(
            IResultFailureLogger resultFailureLogger,
            IQueryHandler<GetEventsQuery, Result<GetEventsQueryResult>> getEventsQueryHandler,
            IQueryHandler<GetEventsNamesQuery, List<EventNameDto>> getEventNamesQueryHandler)
        {
            _resultFailureLogger = resultFailureLogger;
            _getEventsQueryHandler = getEventsQueryHandler;
            _getEventNamesQueryHandler = getEventNamesQueryHandler;
        }

        /// <summary>
        /// get all events names by company
        /// </summary>
        [HttpGet]
        [Route("names")]
        [Authorize(Policy = "Company")]
        public async Task<IActionResult> GetEventNames()
        {
            var result = await _getEventNamesQueryHandler.Handle(new GetEventsNamesQuery());

            if (!result.Any()) return NoContent();

            return Ok(result);
        }

        /// <summary>
        /// Endpoint for browsing available visits for the client.
        /// </summary>
        /// <param name="day">date which query will filter by, must be greater than now utc.</param>
        /// <param name="cityId">cityId to search</param>
        /// <param name="lastVisitId">last visit id that will be use for filtering</param>
        /// <param name="categoryId">category id</param>
        /// <param name="timeOfDay">1: morning; 2: afternoon; 3: evening</param>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents(
            [FromQuery] DateTime day, 
            [FromQuery] int cityId, 
            [FromQuery] int? lastVisitId, 
            [FromQuery] int? categoryId, 
            [FromQuery] int? timeOfDay)
        {

            var query = new GetEventsQuery
            {
                Day = day,
                CityId = cityId,
                LastVisitId = lastVisitId,
                CategoryId = categoryId,
                TimeOfDay = timeOfDay
            };

            var result = await _getEventsQueryHandler.Handle(query);

            if (result.IsFailure)
            {
                _resultFailureLogger.LogFailureError(result, query);
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
}