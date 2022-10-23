using System;

namespace MeetAndGo.Data.Models
{
    public class HistoricalVisit
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public decimal Price { get; set; }
        public int MaxPersons { get; set; }
        public int EventId { get; set; }
        public int TimeOfDay { get; set; } // 1 - morning, 2 - afternoon, 3 - evening
        public string UserId { get; set; }
    }
}