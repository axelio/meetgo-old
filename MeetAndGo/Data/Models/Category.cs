using System.Collections.Generic;

namespace MeetAndGo.Data.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Event> Events { get; set; }
    }

    // 1: Sport
    // 2: Art
    // 3: Fun
    // 4: Gastro (pubs & restaurants)
    // 5: Relax
    // 6: Tourism
}