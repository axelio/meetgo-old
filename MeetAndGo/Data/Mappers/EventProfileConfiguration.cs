using AutoMapper;
using MeetAndGo.Data.Dto;
using MeetAndGo.Data.Models;

namespace MeetAndGo.Data.Mappers
{
    public class EventProfileConfiguration : Profile
    {
        public EventProfileConfiguration()
        {
            CreateMap<Event, EventDto>()
                .ForMember(e => e.Address, opts => opts.MapFrom(e => e.Address));

            CreateMap<EventDto, EventWithVisitsDto>()
                .ForMember(e => e.Visits, opts => opts.Ignore())
                .ForMember(e => e.Order, opts => opts.Ignore());
        }
    }
}