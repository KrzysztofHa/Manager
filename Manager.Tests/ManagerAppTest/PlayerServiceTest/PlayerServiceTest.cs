using FluentAssertions;
using Manager.App;
using Manager.App.Abstract;
using Manager.Domain.Entity;
using Manager.Helpers;
using Moq;

namespace Manager.Tests.ManagerAppTest.PlayerServiceTest;

public class PlayerServiceTest
{
    public List<Player> PlayerList { get; set; }

    public PlayerServiceTest()
    {
        PlayerList = new List<Player>();
        for (int i = 1; i <= 10; i++)
        {
            PlayerList.Add(new Player { IsActive = true, FirstName = "Player" + i + 1 });
        }
    }

    [Fact]
    public void CantGetListofActivePlayer()
    {
        //Arrange
        IPlayerService playerService = new PlayerService();
        playerService.AddRangeItems(PlayerList);

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
        playerService.AddRangeItems(PlayerList);
        string testString = "1";
        //Act
        var resultListFindPlayer = playerService.SearchPlayer(testString);

        //Assert
        resultListFindPlayer.Should().NotBeEmpty();        
        resultListFindPlayer.Count.Should().Be(10);
    }
}