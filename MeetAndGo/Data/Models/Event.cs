using System;
using System.Collections.Generic;

namespace MeetAndGo.Data.Models
{
    public class Event
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public int? DurationInMinutes { get; set; } 
        public string UserId { get; set; }
        public bool RequiresConfirmation { get; set; }
        public int? CategoryId { get; set; }
        public int AddressId { get; set; }
        public int Kind { get; set; } 
        public List<Visit> Visits { get; set; }
        public User Company { get; set; }
        public Category Category { get; set; }
        public Address Address { get; set; }
    }
}