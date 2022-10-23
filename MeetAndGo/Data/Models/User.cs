using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MeetAndGo.Data.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public CompanySettings CompanySettings { get; set; }
        public List<Event> Events { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
