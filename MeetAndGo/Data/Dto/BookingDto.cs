using System;

namespace MeetAndGo.Data.Dto
{
    public class ClientBookingDto
    {
        public int Id { get; set; }
        public DateTime VisitStartDate { get; set; }
        public string EventName { get; set; }
        public int EventKind { get; set; }
        public string CompanyName { get; set; }
        public decimal Price { get; set; }
        public int MaxPersons { get; set; }
        public bool IsConfirmed { get; set; }
        public int BookingsNumber { get; set; }
        public string Code { get; set; }
    }

    public class CompanyBookingDto
    {
        public int Id { get; set; }
        public string CustomerMail { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public bool IsConfirmed { get; set; }
        public string Code { get; set; }
    }

    public class DeletedBookingDto
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public bool IsConfirmed { get; set; }
        public string UserId { get; set; }
        public int VisitId { get; set; }
    }
}