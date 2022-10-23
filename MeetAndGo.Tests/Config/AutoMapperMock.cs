using AutoMapper;
using MeetAndGo.Data.Mappers;

namespace MeetAndGo.Tests.Config
{
    public static class AutoMapperMock
    {
        public static IMapper GetAutoMapper()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AddressProfileConfiguration());
                mc.AddProfile(new BookingProfileConfiguration());
                mc.AddProfile(new EventProfileConfiguration());
                mc.AddProfile(new VisitProfileConfiguration());
            });
            return mappingConfig.CreateMapper();
        }
    }
}
