using Manager.App.Abstract;
using Manager.App.Common;
using Manager.Domain.Entity;

namespace Manager.App.Concrete;

public class SinglePlayerDuelService : BaseService<SinglePlayerDuel>, ISinglePlayerDuelService
{
    public bool StartSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel)
    {
        AddItem(singlePlayerDuel);
        SaveList();
        return true;
    }
    public bool EndSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel)
    {
        singlePlayerDuel.EndGame = DateTime.Now;
        UpdateItem(singlePlayerDuel);
        SaveList();
        return true;
    }

    public bool UpdateSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel)
    {
        UpdateItem(singlePlayerDuel);
        SaveList();
        return true;
    }
}
