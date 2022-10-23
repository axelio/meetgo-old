using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetAndGo.Data;
using MeetAndGo.Data.Dto;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace MeetAndGo.Infrastructure.Handlers.Queries.EventQueries
{
    public class GetEventsNamesQuery : IQuery
    {
        public sealed class GetEventsNamesQueryHandler : IQueryHandler<GetEventsNamesQuery, List<EventNameDto>>
        {
            private readonly MeetGoDbContext _dbContext;
            private readonly IIdentityProvider _identityProvider;

            public GetEventsNamesQueryHandler(MeetGoDbContext dbContext, IIdentityProvider identityProvider)
            {
                _dbContext = dbContext;
                _identityProvider = identityProvider;
            }

            public async Task<List<EventNameDto>> Handle(GetEventsNamesQuery query)
            {
                var companyId = _identityProvider.GetUserIdFromClaims();

                return await _dbContext.Events
                    .AsNoTracking()
                    .Where(e => e.UserId == companyId)
                    .Select(e => new EventNameDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Kind = e.Kind
                    })
                    .ToListAsync();
            }
        }
    }
}