using Manager.App.Abstract;
using Manager.App.Common;
using Manager.Domain.Entity;

namespace Manager.App.Concrete;

public class SinglePlayerDuelService : BaseService<SinglePlayerDuel>, ISinglePlayerDuelService
{
    public void StartSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel)
    {
        AddItem(singlePlayerDuel);
        SaveList();
    }
    public void EndSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel)
    {
        singlePlayerDuel.EndGame = DateTime.Now;
        UpdateItem(singlePlayerDuel);
        SaveList();
    }

    public void UpdateSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel)
    {
        UpdateItem(singlePlayerDuel);
        SaveList();
    }

    public List<SinglePlayerDuel> GetAllSinglePlayerDuel()
    {
        return GetAllItem();
    }
}
