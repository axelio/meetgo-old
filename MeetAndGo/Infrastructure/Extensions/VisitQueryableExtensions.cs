using System;
using System.Linq;
using MeetAndGo.Data.Models;

namespace MeetAndGo.Infrastructure.Extensions
{

    public static class VisitQueryableExtensions
    {
        public static IQueryable<Visit> ApplyLastIdCondition(this IQueryable<Visit> @this, int? lastId)
            => lastId.HasValue ? @this.Where(v => v.Id > lastId.Value) : @this;

        public static IQueryable<Visit> ApplyTimeOfDayCondition(this IQueryable<Visit> @this, int? timeOfDay)
            => timeOfDay.HasValue ? @this.Where(v => v.TimeOfDay == timeOfDay) : @this;

        public static IQueryable<Visit> ApplyCategoryIdCondition(this IQueryable<Visit> @this, int? categoryId)
            => categoryId.HasValue ? @this.Where(v => v.Event.CategoryId == categoryId) : @this;

        public static IQueryable<Visit> ApplyDateCondition(this IQueryable<Visit> @this, DateTime queryDate)
        {
            var now = DateTimeOffset.Now;

            return now.Date == queryDate
                ? @this.Where(v => v.StartDate >= now && v.StartDate.Date == queryDate.Date)
                : @this.Where(v => v.StartDate.Date == queryDate.Date);
        }
    }
}
