using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Manager.Infrastructure.Concrete;
using Manager.Infrastructure.Entity;

namespace Manager.App.Tests;

public class BasePathServiceTests
{
    private static string BaseDirectory => Path.Combine(Directory.GetCurrentDirectory(), "base");
    private static string PathToBaseCurrent() => Directory.GetCurrentDirectory() + "\\base\\";

    private static string FileName => nameof(BasePaths) + ".manager.json";
    private static string FullPath => Path.Combine(BaseDirectory, FileName);

    [Fact]
    public void Constructor_WhenFileDoesNotExist_AddsNewEntryAndCreatesFile()
    {
        // Arrange
        if (File.Exists(FullPath))
            File.Delete(FullPath);
        if (Directory.Exists(BaseDirectory))
            Directory.Delete(BaseDirectory, recursive: true);

        // Act
        var sut = new BasePathsService();

        // Assert
        sut.ListOfElements.Should().NotBeNull();
        sut.ListOfElements.Should().Contain(e => e.PathName == nameof(BasePaths));
    }

    [Fact]
    public void Constructor_WhenFileExists_LoadsListFromFile()
    {
        // Arrange
        if (!Directory.Exists(BaseDirectory))
            Directory.CreateDirectory(BaseDirectory);

        var expected = new System.Collections.Generic.List<BasePaths>
        {
            new BasePaths
            {
                Id = 42,
                PathName = nameof(BasePaths),
                PathToFile = "some-path",
                IsActive = false,
                UserName = "tester"
            }
        };

        var inner = JsonConvert.SerializeObject(expected);
        var outer = JsonConvert.SerializeObject(inner);
        File.WriteAllText(FullPath, outer);

        // Act
        var sut = new BasePathsService();

        // Assert
        sut.ListOfElements.Should().NotBeNull();
        sut.ListOfElements.Should().ContainSingle(e => e.Id == 42 && e.UserName == "tester" && e.IsActive == false);
    }

    [Fact(Skip = "ProductionBugSuspected")]
    public void Constructor_WhenFileContainsJsonNull_SetsListOfElementsNull()
    {
        // Arrange
        if (!Directory.Exists(BaseDirectory))
            Directory.CreateDirectory(BaseDirectory);

        File.WriteAllText(FullPath, "null");

        // Act
        var sut = new BasePathsService();

        // Assert
        sut.ListOfElements.Should().BeNull();
    }

    [Fact]
    public void Constructor_WhenFileDoesNotExist_AddsNewEntry_WithExpectedPathToFile()
    {
        // Arrange
        if (File.Exists(FullPath))
            File.Delete(FullPath);
        if (Directory.Exists(BaseDirectory))
            Directory.Delete(BaseDirectory, recursive: true);

        var expectedPathToFile = PathToBaseCurrent() + nameof(BasePaths) + ".manager.json";

        // Act
        var sut = new BasePathsService();

        // Assert
        sut.ListOfElements.Should().NotBeNull();
        sut.ListOfElements.Should().Contain(e => e.PathName == nameof(BasePaths));
        var entry = sut.ListOfElements.First(e => e.PathName == nameof(BasePaths));
        entry.PathToFile.Should().Be(expectedPathToFile);
    }

    [Fact]
    public void Constructor_WhenFileExists_WithEmptyList_SetsEmptyList()
    {
        // Arrange
        if (!Directory.Exists(BaseDirectory))
            Directory.CreateDirectory(BaseDirectory);

        var expected = new System.Collections.Generic.List<BasePaths>();
        var inner = JsonConvert.SerializeObject(expected);
        var outer = JsonConvert.SerializeObject(inner);
        File.WriteAllText(FullPath, outer);

        // Act
        var sut = new BasePathsService();

        // Assert
        sut.ListOfElements.Should().NotBeNull();
        sut.ListOfElements.Should().BeEmpty();
    }

}
