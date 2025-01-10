using FluentAssertions;
using Manager.Domain.Entity;
using Manager.Infrastructure.Common;

namespace Manager.Tests.InfrastructureTest;

public class BaseOperationServiceUnitTest
{
    [Fact]
    public void CanLoadListINBase()
    {
        //Arrange
        Directory.SetCurrentDirectory(@"c:\temp\");
        BaseOperationService<Player> iBaseService = new BaseOperationService<Player>();

        //Act
        var resultiBaseService = iBaseService.LoadListInBase();

        //Assert
        resultiBaseService.Should().BeEmpty();
    }

    //[Fact]
    //public void CanSaveListToBase()
    //{
    //    //Arrange
    //    List<Player> playerList = new List<Player>();
    //    for (int i = 1; i <= 10; i++)
    //    {
    //        playerList.Add(new Player { Id = i + 1, IsActive = true, FirstName = "Player" + i + 1, Country = "Poland" });
    //    }

    //    Directory.SetCurrentDirectory(@"c:\temp\");
    //    BaseOperationService<Player> iBaseService = new BaseOperationService<Player>();
    //    iBaseService.ListOfElements = playerList;

    //    //Act
    //    var resultiBaseService = iBaseService.SaveListToBase();

    //    //Assert
    //    Assert.True(resultiBaseService);
    //}
}