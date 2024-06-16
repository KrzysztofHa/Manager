using Manager.Domain.Entity;

namespace Manager.App.Abstract;

internal interface ISinglePlayerDuelService
{
    void StartSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);
    void UpdateSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);
    void EndSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);
    List<SinglePlayerDuel> GetAllSinglePlayerDuel();

}
