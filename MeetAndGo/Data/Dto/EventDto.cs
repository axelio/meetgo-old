using System.Collections.Generic;

namespace MeetAndGo.Data.Dto
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationInMinutes { get; set; }
        public string PictureUrl { get; set; }
        public bool RequiresConfirmation { get; set; }
        public int Kind { get; set; }
        public AddressDto Address { get; set; }
    }

    public class EventNameDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Kind { get; set; }
    }

    public class EventWithVisitsDto : EventDto
    {
        public List<VisitDisplayDto> Visits { get; set; }
        public int Order { get; set; }
    }

    public class GetEventsQueryResult
    {
        public Dictionary<int, EventWithVisitsDto> Events { get; set; }
        public int? LastVisitId { get; set; }
        public int VisitsCount { get; set; }
    }
}
