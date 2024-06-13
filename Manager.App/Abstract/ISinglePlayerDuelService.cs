using Manager.Domain.Entity;

namespace Manager.App.Abstract;

internal interface ISinglePlayerDuelService
{
    public bool StartSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);    
    public bool UpdateSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);
    public bool EndSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);
}
