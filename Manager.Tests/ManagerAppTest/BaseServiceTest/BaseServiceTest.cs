using Manager.App.Common;
using Manager.Domain.Entity;

namespace Manager.Tests.ManagerAppTest.BaseServiceTest;

public class BaseServiceTest
{
    public List<Player> SomListOfElements { get; set; }
    public BaseService<Player> baseService = new BaseService<Player>();

    public BaseServiceTest()
    {
        SomListOfElements = new List<Player>();
        for (int i = 1; i <= 10; i++)
        {
            SomListOfElements.Add(new Player { Id = i, IsActive = true, FirstName = "Player" + i + 1 });
        }
        baseService.AddItem(SomListOfElements.First(p => p.IsActive = true));
    }

    [Fact]
    public void CantGetPlayerOfID()
    {
        //Arrange

        //Act
        var elementOfId = baseService.GetItemById(1);

        //Assert
        Assert.NotNull(elementOfId);
        Assert.True(elementOfId is Player);
        Assert.Equal("Player11", SomListOfElements[0].FirstName);
    }
}