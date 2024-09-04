using Manager.App.Abstract;

namespace Manager.App.Managers.Helpers.GamePlaySystem;

public class GroupSystem
{
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;

    public GroupSystem(PlayersToTournament playersToTournament, ISinglePlayerDuelManager singlePlayerDuelManager)
    {
        _singlePlayerDuelManager = singlePlayerDuelManager;
    }
}