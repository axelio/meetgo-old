using System.Collections.Generic;

namespace MeetAndGo.Data.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Address> Addresses { get; set; }
    }
}