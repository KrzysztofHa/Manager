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
        var resultStartGame = _singlePlayerDuelService.StartSinglePlayerDuel(duel);

        //Assert
        resultStartGame.Should().BeTrue();
        duel.Id.Should().Be(1).Should().NotBeNull();
        duel.StartGame.Should().BeBefore(DateTime.Now);
    }
}
