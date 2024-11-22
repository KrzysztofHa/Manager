using Manager.App.Abstract;
using Manager.Domain.Entity;

namespace Manager.App.Managers.Helpers.GamePlaySystem;

public class GroupSystem
{
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;
    private readonly ITournamentsService _tournamentService;

    public GroupSystem(Tournament tournament, ITournamentsService tournamentsService, ISinglePlayerDuelManager singlePlayerDuelManager)
    {
        _singlePlayerDuelManager = singlePlayerDuelManager;
        _tournamentService = tournamentsService;
    }
}