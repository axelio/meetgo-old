using FluentAssertions;
using MeetAndGo.Infrastructure.Extensions;
using System.Collections.Generic;
using Xunit;

namespace MeetAndGo.Tests.Infrastructure
{

    public class EnumerableExtensionsTests
    {
        [Fact]
        public void ShouldReturnTrueForEmpty()
        {
            var emptyList = new List<string>();
            emptyList.IsNullOrEmpty().Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnTrueForNukl()
        {
            List<int> list = null;
            list.IsNullOrEmpty().Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseForPopulatedList()
        {
            var list = new List<string> { "1", "a" };
            list.IsNullOrEmpty().Should().BeFalse();
        }
    }
}