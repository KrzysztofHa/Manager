using Manager.Domain.Entity;

namespace Manager.App.Abstract;

internal interface ISinglePlayerDuelService
{
    bool StartSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);
    bool UpdateSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);
    bool EndSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);
    List<SinglePlayerDuel> GetAllSinglePlayerDuel();

}
