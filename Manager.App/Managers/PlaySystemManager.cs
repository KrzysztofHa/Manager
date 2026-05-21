using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers.Helpers;
using Manager.App.Managers.Helpers.GamePlaySystem;
using Manager.App.Managers.Helpers.TournamentGamePlaySystem;
using Manager.App.Managers.Helpers.TypeOfplayPlaySystem;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Managers;

public class PlaySystemManager
{
    public readonly TypeOfPlaySystem ListGamePlaySystem = new TypeOfPlaySystem();
    private readonly IPlayerManager _playerManager;
    private readonly ITournamentsManager _tournamentsManager;
    private readonly IPlayerService _playerService;
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;
    private readonly MenuActionService _actionService;
    private readonly PlaySystemManager _playSystemManager;
    private PlayersToTournament _playersToTournament;
    public Tournament Tournament { get; }

    public PlaySystemManager(ITournamentsManager tournamentsManager, ISinglePlayerDuelManager singlePlayerDuelManager, PlayersToTournament playersToTournament, IPlayerService playerService, IPlayerManager playerManager)
    {
        _singlePlayerDuelManager = singlePlayerDuelManager;
        _playerService = playerService;
        _playerManager = playerManager;
        _actionService = new MenuActionService();
        _tournamentsManager = tournamentsManager;
        _playSystemManager = this;
        _playersToTournament = playersToTournament;
    }

    public PlaySystems? CreateNewPlaySystem(Tournament tournament)
    {
        PlaySystems? playSystem = null;
        if (tournament.GamePlaySystem == "Group" && !string.IsNullOrEmpty(tournament.GamePlaySystem))
        {
            playSystem = new GroupPlaySystem(tournament, _tournamentsManager, _singlePlayerDuelManager, _playersToTournament, _playerService, _playerManager);
        }
        else if (!string.IsNullOrEmpty(tournament.GamePlaySystem))
        {
            playSystem = new TwoKOPlaySystem(tournament, _tournamentsManager, _singlePlayerDuelManager, _playersToTournament, _playerService, _playerManager);
        }

        return playSystem;
    }
}