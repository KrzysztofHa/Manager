using FluentAssertions;
using Manager.Domain.Entity;
using Manager.Infrastructure.Concrete;

namespace Manager.Tests.InfrastructureTest;

public class BasePathUnitTest
{
    [Fact]
    public void CanAddNewEntryInPathsList()
    {
        //Arrange
        var pathName = typeof(Player).Name;
        var basePathsService = new BasePathsService();

        //Act
        var resultbasePathService = basePathsService.AddNewEntryToPathsList(pathName);
        //Assert
        resultbasePathService.Should().NotBeNull();
        resultbasePathService.Should().BeOfType<string>();
        Assert.Equal(resultbasePathService, basePathsService.ListOfElements[1].PathToFile);
    }

    [Fact]
    public void CanGetPathToFileOfTypeName()
    {
        //Arrange

        var pathName = typeof(Player).Name;
        var basePathsService = new BasePathsService();

        //Act 
        var resultBasePathsService = basePathsService.GetPathToFileOfTypeName(pathName);

        //Assert
        Assert.NotNull(resultBasePathsService);
        Assert.Equal(resultBasePathsService, basePathsService.ListOfElements[1].PathToFile);
    }


}
