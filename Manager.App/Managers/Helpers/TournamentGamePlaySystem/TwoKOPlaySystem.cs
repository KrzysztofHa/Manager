using Manager.App.Abstract;
using Manager.App.Managers.Helpers.TournamentGamePlaySystem;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Managers.Helpers.GamePlaySystem;

public class TwoKOPlaySystem : PlaySystems
{
    protected readonly ITournamentsManager _tournamentsManager;
    protected readonly ISinglePlayerDuelManager _singlePlayerDuelManager;

    public TwoKOPlaySystem(Tournament tournament, ITournamentsManager tournamentsManager, ISinglePlayerDuelManager singlePlayerDuelManager) : base(tournament)
    {
        _tournamentsManager = tournamentsManager;
        _singlePlayerDuelManager = singlePlayerDuelManager;
    }

    public override void StartTournament()
    {
        throw new NotImplementedException();
    }

    public override string ViewTournamentBracket(PlayersToTournament playersToTournament)
    {
        var formatText = string.Empty;
        if (playersToTournament.ListPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.TwoKO)))
        {
            formatText = $"\n\rStart List 2KO System\n\r\n\r";
            foreach (var player in playersToTournament.ListPlayersToTournament.OrderBy(p => p.Position))
            {
                formatText += $" {playersToTournament.ListPlayersToTournament.IndexOf(player) + 1}. {player.FirstName} {player.LastName} {player.Country}\n\r";
            }
        }
        return formatText;
    }
}