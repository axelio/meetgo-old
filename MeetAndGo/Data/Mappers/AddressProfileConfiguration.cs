using AutoMapper;
using MeetAndGo.Data.Dto;
using MeetAndGo.Data.Models;

namespace MeetAndGo.Data.Mappers
{
    public class AddressProfileConfiguration : Profile
    {
        public AddressProfileConfiguration()
        {
            CreateMap<Address, AddressDto>()
                .ForMember(dto => dto.Longitude, opts => opts.MapFrom(a => a.Location.X))
                .ForMember(dto => dto.Latitude, opts => opts.MapFrom(a => a.Location.Y));
        }
    }
}