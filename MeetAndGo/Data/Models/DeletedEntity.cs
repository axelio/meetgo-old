using System;

namespace MeetAndGo.Data.Models
{
    public class DeletedEntity
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string Type { get; set; }
        public int IdOrig { get; set; }
        public string JsonEntity { get; set; }
    }
}