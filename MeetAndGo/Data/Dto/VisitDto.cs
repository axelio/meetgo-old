using System;
using System.Collections.Generic;

namespace MeetAndGo.Data.Dto
{
    public class VisitDisplayDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public decimal Price { get; set; }
        public int MaxPersons { get; set; }
        public int BookingsNumber { get; set; }
    }

    public class VisitWithEventDto : VisitDisplayDto
    {
        public EventDto Event { get; set; }
    }

    public class CompanyVisitDisplayDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public string EventName { get; set; }
        public int EventKind { get; set; }
        public decimal Price { get; set; }
        public int BookingsNumber { get; set; }
        public int MaxPersons { get; set; }
        public bool RequiresConfirmation { get; set; }
        public List<CompanyBookingDto> Bookings { get; set; }
    }

    public class DeletedVisitDto
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public decimal Price { get; set; }
        public int MaxPersons { get; set; }
        public int EventId { get; set; }
        public int CityId { get; set; }
    }
}