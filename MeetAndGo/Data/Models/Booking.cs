using System;

namespace MeetAndGo.Data.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public bool IsConfirmed { get; set; }
        public string UserId { get; set; }
        public int VisitId { get; set; }
        public Visit Visit { get; set; }
        public User Customer { get; set; }
        public string Code { get; set; }
    }
}