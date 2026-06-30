using FluentAssertions;
using Xunit;
using Manager.Domain.Entity;

namespace Manager.App.Tests;

public class MenuActionTests
{
    [Fact]
    public void Constructor_ValidParameters_PropertiesAreSet()
    {
        // Arrange
        int id = 42;
        string name = "TestName";
        string menuName = "TestMenu";

        // Act
        var action = new MenuAction(id, name, menuName);

        // Assert
        action.Id.Should().Be(id);
        action.Name.Should().Be(name);
        action.MenuName.Should().Be(menuName);
    }
}
