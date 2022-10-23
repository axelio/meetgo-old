using System;

namespace MeetAndGo.Data.Models
{
    public class HistoricalBooking
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public bool IsConfirmed { get; set; }
        public string UserId { get; set; }
        public int VisitId { get; set; }
    }
}