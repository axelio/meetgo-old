using FluentAssertions;
using MeetAndGo.Infrastructure.Extensions;
using System;
using Xunit;

namespace MeetAndGo.Tests.Infrastructure
{
    public class DateTimeOffsetExtensionsTests
    {
        [Fact]
        public void ShouldCreateFriendlyDate()
        {
            var date = new DateTimeOffset(new DateTime(2021, 12, 30, 12, 30, 00), TimeSpan.FromHours(1));
            var friendly = date.ToFriendlyString();
            friendly.Should().Be("30-12-2021 12:30");
        }
    }
}