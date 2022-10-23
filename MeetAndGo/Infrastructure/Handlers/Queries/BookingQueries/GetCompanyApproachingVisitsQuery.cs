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
    public class GetCompanyApproachingVisitsQuery : IQuery { }
    public sealed class GetCompanyApproachingVisitsQueryHandler : IQueryHandler<GetCompanyApproachingVisitsQuery, List<CompanyVisitDisplayDto>>
    {
        private readonly IMapper _mapper;
        private readonly IIdentityProvider _identityProvider;
        private readonly MeetGoDbContext _dbContext;

        public GetCompanyApproachingVisitsQueryHandler(
            IMapper mapper,
            IIdentityProvider identityProvider,
            MeetGoDbContext dbContext)
        {
            _mapper = mapper;
            _identityProvider = identityProvider;
            _dbContext = dbContext;
        }
        public async Task<List<CompanyVisitDisplayDto>> Handle(GetCompanyApproachingVisitsQuery query)
        {
            var userId = _identityProvider.GetUserIdFromClaims();

            var todayDate = DateTimeOffset.Now.Date;

            var dbQuery = _dbContext.Visits
                .AsNoTracking()
                .Where(v => v.Event.UserId == userId)
                .Where(v => v.StartDate.Date >= todayDate);

            return await _mapper.ProjectTo<CompanyVisitDisplayDto>(dbQuery).ToListAsync();
        }
    }

}