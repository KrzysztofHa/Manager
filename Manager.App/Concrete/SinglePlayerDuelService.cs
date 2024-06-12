using Manager.App.Abstract;
using Manager.App.Common;
using Manager.Domain.Entity;

namespace Manager.App.Concrete;

public class SinglePlayerDuelService : BaseService<SinglePlayerDuel>, ISinglePlayerDuelService
{
    public bool StartSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel)
    {
        singlePlayerDuel.StartGame = DateTime.Now;
        AddItem(singlePlayerDuel);
        return true;
    }

    public bool SaveRack(Rack rack)
    {
        throw new NotImplementedException();
    }

    public bool UpdateDuel(SinglePlayerDuel singlePlayerDuel)
    {
        throw new NotImplementedException();
    }

    public bool EndSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel)
    {
        throw new NotImplementedException();
    }
}
