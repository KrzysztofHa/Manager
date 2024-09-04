using FluentAssertions;
using Manager.App;
using Manager.App.Abstract;
using Manager.Domain.Entity;

namespace Manager.Tests.ManagerAppTest.PlayerServiceTest;

public class PlayerServiceTest
{
    public List<Player> PlayerList { get; set; }

    public PlayerServiceTest()
    {
        PlayerList = new List<Player>();
        for (int i = 1; i <= 10; i++)
        {
            PlayerList.Add(new Player { Id = i, IsActive = true, FirstName = "Player" + i + 1 });
        }
    }

    [Fact]
    public void CantGetListofActivePlayer()
    {
        //Arrange
        IPlayerService playerService = new PlayerService();
        PlayerList.ForEach(player => playerService.AddItem(player));

        //Act
        var resulActivePlayertList = playerService.ListOfActivePlayers();

        //Assert
        resulActivePlayertList.Should().NotBeEmpty();
        Assert.Equal(resulActivePlayertList[0], PlayerList[0]);
    }

    [Fact]
    public void CantGetListPlayersOfStringInput()
    {
        //Arrang
        IPlayerService playerService = new PlayerService();
        PlayerList.ForEach(player => playerService.AddItem(player));
        string testString = "1";
        //Act
        var resultListFindPlayer = playerService.SearchPlayer(testString);

        //Assert
        resultListFindPlayer.Should().NotBeEmpty();
        resultListFindPlayer[0].Id.Should().Be(10);
        resultListFindPlayer.Count.Should().Be(10);
    }
}