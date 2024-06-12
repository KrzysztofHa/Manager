using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers;
using Manager.Domain.Entity;
using Moq;
using Xunit.Abstractions;


namespace Manager.Tests.ManagerAppTest.PlayerManagerTest;

public class PlayerManagerTest
{
    private readonly ITestOutputHelper output;
    public List<Player> PlayerList { get; set; }

    public PlayerManagerTest(ITestOutputHelper output)
    {
        this.output = output;
        PlayerList = new List<Player>();
        for (int i = 1; i <= 10; i++)
        {
            PlayerList.Add(new Player { Id = i, IsActive = true, FirstName = "Player" + i + 1 });
        }
    }

    [Fact]
    public void CanAddNewPlayer()
    {
        //Arrange
        var mockIService = new Mock<IPlayerService>();
        mockIService.Setup(p => p.AddItem(It.IsAny<Player>()));
        mockIService.Setup(p => p.SaveList());
        mockIService.Setup(p => p.GetAllItem()).Returns(PlayerList);


        var mockIUserService = new Mock<IUserService>();
        var playerManager = new PlayerManager(new MenuActionService(), mockIService.Object, mockIUserService.Object);


        //Act
        var returnPlayerManager = playerManager.AddNewPlayer();

        Assert.NotNull(output);
    }

}
