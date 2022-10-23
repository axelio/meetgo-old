using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace MeetAndGo.Data.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string District { get; set; }
        public string Website { get; set; }
        public Point Location { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public List<Event> Events { get; set; }
    }
}