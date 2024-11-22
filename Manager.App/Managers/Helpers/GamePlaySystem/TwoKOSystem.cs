using Manager.App.Abstract;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Managers.Helpers.GamePlaySystem;

public class TwoKOSystem
{
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;
    private readonly ITournamentsService _tournamentService;
    private readonly PlayersToTournament _playersToTournament = new PlayersToTournament();

    public TwoKOSystem(Tournament tournament, ITournamentsService tournamentService, ISinglePlayerDuelManager singlePlayerDuelManager)
    {
        _singlePlayerDuelManager = singlePlayerDuelManager;
        _tournamentService = tournamentService;
    }
}