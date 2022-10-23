using System.Threading.Tasks;
using AutoMapper;
using MeetAndGo.Data.Mappers;
using Xunit;

namespace MeetAndGo.Tests
{
    public class AutomapperTests
    {
        [Fact]
        public void TestAutomapper()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AddressProfileConfiguration());
                mc.AddProfile(new BookingProfileConfiguration());
                mc.AddProfile(new EventProfileConfiguration());
                mc.AddProfile(new VisitProfileConfiguration());
            });

            mappingConfig.AssertConfigurationIsValid();
        }
    }
}