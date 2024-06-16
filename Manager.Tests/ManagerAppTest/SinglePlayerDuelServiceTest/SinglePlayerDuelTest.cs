using FluentAssertions;
using Manager.App.Concrete;
using Manager.Domain.Entity;

namespace Manager.Tests.ManagerAppTest.SinglePlayerDuelTest;

public class SinglePlayerDuelTest
{
    private readonly SinglePlayerDuelService _singlePlayerDuelService;

    public SinglePlayerDuelTest()
    {
        _singlePlayerDuelService = new SinglePlayerDuelService();
    }
    [Fact]
    public void CanStartSinglePlayerDuel()
    {
        //Arrenge
        var duel = new SinglePlayerDuel() { IdFirstPlayer = 1, IdSecondPlayer = 2, RaceTo = 4, TypeNameOfGame = "8 balls", ModifiedById = 1 };


        //Act
        _singlePlayerDuelService.StartSinglePlayerDuel(duel);

        //Assert        
        duel.Id.Should().Be(1).Should().NotBeNull();
        duel.StartGame.Should().BeBefore(DateTime.Now);
    }
    [Fact]
    public void CanUpdateSinglePlayerDuel()
    {
        //Arrange
        CanStartSinglePlayerDuel();
        //CanStartSinglePlayerDuel();
        var duel = _singlePlayerDuelService.GetItemById(1);
        duel.ScoreFirstPlayer = 4;

        //Act
        _singlePlayerDuelService.UpdateSinglePlayerDuel(duel);

        //Assert        
        _singlePlayerDuelService.GetItemById(1).ScoreFirstPlayer.Should().Be(4);
    }


    [Fact]
    public void CanEndSinglePlayerDuel()
    {
        // Arrange
        CanUpdateSinglePlayerDuel();
        var duel = _singlePlayerDuelService.GetItemById(1);


        //Act
        _singlePlayerDuelService.EndSinglePlayerDuel(duel);

        //Assert        
        _singlePlayerDuelService.GetItemById(1).EndGame.Should().BeAfter(duel.StartGame);

    }
}
