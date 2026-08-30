using FluentAssertions;
using Manager.App.Concrete;
using Manager.Domain.Entity;

namespace Manager.Tests.ManagerAppTest.SinglePlayerDuelTest;

public class SinglePlayerDuelTest
{
    private readonly SinglePlayerDuelService _singlePlayerDuelService;
    private int duelId = 0;
    private List<SinglePlayerDuel> _singlePlayerDuels = new List<SinglePlayerDuel>();

    public SinglePlayerDuelTest()
    {
        _singlePlayerDuelService = new SinglePlayerDuelService();
        _singlePlayerDuels = _singlePlayerDuelService.GetAllItem();
    }

    [Fact]
    public void CanStartSinglePlayerDuel()
    {
        //Arrenge
        var duel = new SinglePlayerDuel() { IdSecondPlayer = 2, RaceTo = 4, TypeNameOfGame = "8 balls", ModifiedById = 1 };
        duelId = _singlePlayerDuelService.GetAllSinglePlayerDuel().Max(d => d.Id) + 1;

        //Act
        _singlePlayerDuelService.StartSinglePlayerDuel(duel);

        //Assert
        duel.Id.Should().Be(duelId).Should().NotBeNull();
        duel.IdSecondPlayer.Should().Be(2);
        duel.StartGame.Should().BeBefore(DateTime.Now);
    }

    [Fact]
    public void CanUpdateSinglePlayerDuel()
    {
        //Arrange
        CanStartSinglePlayerDuel();
        //CanStartSinglePlayerDuel();
        var duel = _singlePlayerDuelService.GetItemById(duelId);
        duel.ScoreFirstPlayer = 4;

        //Act
        _singlePlayerDuelService.UpdateSinglePlayerDuel(duel);

        //Assert
        _singlePlayerDuelService.GetItemById(duelId).ScoreFirstPlayer.Should().Be(4);
    }

    [Fact]
    public void CanEndSinglePlayerDuel()
    {
        // Arrange
        CanUpdateSinglePlayerDuel();
        var duel = _singlePlayerDuelService.GetItemById(duelId);

        //Act
        _singlePlayerDuelService.EndSinglePlayerDuel(duel);

        //Assert
        _singlePlayerDuelService.GetItemById(duelId).EndGame.Should().BeAfter(duel.StartGame);
        // clean up list
        _singlePlayerDuelService.RemoveItem(duel);
    }
}