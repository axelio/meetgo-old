using System;

namespace MeetAndGo.Infrastructure.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static string ToFriendlyString(this DateTimeOffset @this) => @this.ToString("dd-MM-yyyy HH:mm");
    }
}