using System;
using System.Collections.Generic;

namespace MeetAndGo.Data.Models
{
    public class Visit
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public decimal Price { get; set; }
        public int MaxPersons { get; set; }
        public int EventId { get; set; }
        public bool IsBooked { get; set; }
        public int CityId { get; set; }
        public int TimeOfDay { get; set; }
        public int BookingsNumber { get; set; }
        public Event Event { get; set; }
        public List<Booking> Bookings { get; set; }

        public void SetAsBooked() => IsBooked = true;
        public void SetAsNotBooked() => IsBooked = false;
        public void IncrementBookingsNumber() => BookingsNumber++;
        public void DecrementBookingsNumber() => BookingsNumber--;
    }
}