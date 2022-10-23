using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MeetAndGo.Data;
using MeetAndGo.Data.Dto;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace MeetAndGo.Infrastructure.Handlers.Queries.BookingQueries
{
    public class GetClientApproachingBookingsQuery : IQuery { }

    public sealed class GetClientApproachingBookingsQueryHandler : IQueryHandler<GetClientApproachingBookingsQuery, List<ClientBookingDto>>
    {
        private readonly IMapper _mapper;
        private readonly MeetGoDbContext _dbContext;
        private readonly IIdentityProvider _identityProvider;

        public GetClientApproachingBookingsQueryHandler(IMapper mapper,
            MeetGoDbContext dbContext,
            IIdentityProvider identityProvider)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _identityProvider = identityProvider;
        }
        public async Task<List<ClientBookingDto>> Handle(GetClientApproachingBookingsQuery query)
        {
            var clientId = _identityProvider.GetUserIdFromClaims();

            var todayDate = DateTimeOffset.Now.Date;

            var dbQuery = _dbContext.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == clientId)
                .Where(b => b.Visit.StartDate.Date >= todayDate);

            return await _mapper.ProjectTo<ClientBookingDto>(dbQuery).ToListAsync();
        }
    }

}