using Manager.App.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Managers.Helpers.GamePlaySystem;

public class TwoKOSystem
{
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;
    private readonly PlayersToTournament _playersToTournament;

    public TwoKOSystem(PlayersToTournament playersToTournament, ISinglePlayerDuelManager singlePlayerDuelManager)
    {
        _singlePlayerDuelManager = singlePlayerDuelManager;
        _playersToTournament = playersToTournament;
    }
}