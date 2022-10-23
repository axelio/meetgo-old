using FluentAssertions;
using MeetAndGo.Infrastructure.Utils;
using Xunit;

namespace MeetAndGo.Tests.Infrastructure
{
    public class PhoneNumberFactoryTests
    {
        [Fact]
        public void ShouldCreateProperNumber()
        {
            var phoneNumber = "796123456";
            var transformed = PhoneNumberFactory.CreatePolishNumber(phoneNumber);
            transformed.Should().Be("+48796123456");
        }
    }
}