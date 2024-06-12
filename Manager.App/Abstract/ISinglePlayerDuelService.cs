using Manager.Domain.Entity;

namespace Manager.App.Abstract;

internal interface ISinglePlayerDuelService
{
    public bool StartSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);
    public bool SaveRack(Rack rack);
    public bool UpdateDuel(SinglePlayerDuel singlePlayerDuel);
    public bool EndSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);
}
