using Manager.App.Abstract;
using Manager.App.Managers.Helpers.TournamentGamePlaySystem;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Managers.Helpers.GamePlaySystem;

public class TwoKOPlaySystem : PlaySystems
{
    public TwoKOPlaySystem(Tournament tournament, ITournamentsManager tournamentsManager, ISinglePlayerDuelManager singlePlayerDuelManager, PlayersToTournament playersToTournament, IPlayerService playerService, IPlayerManager playerManager) : base(tournament, tournamentsManager, singlePlayerDuelManager, playersToTournament, playerService, playerManager)
    {
    }

    public override void AddPlayers()
    {
        var newPlayers = PlayersToTournament.AddPlayersToTournament();

        if (newPlayers.Count > 0 && Tournament.Start != DateTime.MinValue)
        {
        }
    }

    public override void MovePlayer()
    {
        throw new NotImplementedException();
    }

    public override void StartTournament()
    {
        throw new NotImplementedException();
    }

    public override string ViewTournamentBracket()
    {
        var formatText = string.Empty;
        if (PlayersToTournament.ListPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.TwoKO)))
        {
            formatText = $"\n\rStart List 2KO System\n\r\n\r";
            foreach (var player in PlayersToTournament.ListPlayersToTournament.OrderBy(p => p.Position))
            {
                formatText += $" {PlayersToTournament.ListPlayersToTournament.IndexOf(player) + 1}. {player.FirstName} {player.LastName} {player.Country}\n\r";
            }
        }
        return formatText;
    }

    protected override void RemovePlayers(PlayerToTournament playerToRemove)
    {
        if (playerToRemove != null)
        {
            _singlePlayerDuelManager.RemoveTournamentDuel(Tournament, playerToRemove.IdPLayer);
        }
    }
}