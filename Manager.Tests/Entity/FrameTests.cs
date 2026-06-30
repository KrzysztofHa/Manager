using System;
using FluentAssertions;
using Manager.Domain.Entity;
using Xunit;

namespace Manager.App.Tests
{
    public class FrameTests
    {
        [Fact]
        public void StartGame_ReturnsCreatedDateTime()
        {
            // Arrange
            var expectedDate = new DateTime(2024, 6, 1, 12, 30, 0);
            var frame = new Frame { CreatedDateTime = expectedDate };

            // Act
            var result = frame.StartGame;

            // Assert
            result.Should().Be(expectedDate);
        }
    }
}
