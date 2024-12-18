using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Managers.Helpers.TournamentGamePlaySystem;

public abstract class PlaySystems
{
    protected Tournament Tournament { get; }
    protected PlayersToTournament PlayersToTournament { get; }
    protected readonly ISinglePlayerDuelManager _singlePlayerDuelManager;
    protected readonly ITournamentsManager _tournamentsManager;
    private readonly IPlayerService _playerService;
    private readonly IPlayerManager _playerManager;

    protected PlaySystems(Tournament tournament, ITournamentsManager tournamentsManager, ISinglePlayerDuelManager singlePlayerDuelManager, PlayersToTournament playersToTournament, IPlayerService playerService, IPlayerManager playerManager)
    {
        Tournament = tournament;
        _singlePlayerDuelManager = singlePlayerDuelManager;
        PlayersToTournament = playersToTournament;
        _singlePlayerDuelManager = singlePlayerDuelManager;
        _playerService = playerService;
        _playerManager = playerManager;
    }

    protected abstract void RemovePlayers(PlayerToTournament playerToRemove);

    public void RemovePlayerInTournament()
    {
        List<Player> players = new List<Player>();

        if (Tournament.Start != DateTime.MinValue)
        {
            ConsoleService.WriteTitle("");
            ConsoleService.WriteLineErrorMessage("Attention!!!");
            if (!ConsoleService.AnswerYesOrNo("Remember that removing a player may disturb the group structure.\n\r" +
                "Players who are currently playing or have finished the match will not appear on the list."))
            {
                return;
            }
        }

        if (PlayersToTournament.ListPlayersToTournament.Count > 8)
        {
            ConsoleService.WriteLineErrorMessage("You cannot remove a player. \n\rThe minimum number of players is 8.");
            return;
        }

        foreach (var playerToTournament in PlayersToTournament.ListPlayersToTournament)
        {
            var player = _playerService.GetItemById(playerToTournament.IdPLayer);
            bool isPlayerEndDuelOrPlay = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
           .Exists(p => (p.IdFirstPlayer == player.Id || p.IdSecondPlayer == player.Id) && (p.StartGame != DateTime.MinValue && p.Interrupted == DateTime.MinValue) || p.EndGame != DateTime.MinValue);

            if (player != null && !isPlayerEndDuelOrPlay)
            {
                players.Add(player);
            }
        }

        if (players.Count > 0)
        {
            var player = _playerManager.SearchPlayer("Remowe Player", players);
            if (player == null)
            {
                return;
            }
            else
            {
                var playerToRemove = PlayersToTournament.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == player.Id);

                if (Tournament.Start != DateTime.MinValue)
                {
                    RemovePlayers(playerToRemove);
                    PlayersToTournament.RemovePlayerInTournament(playerToRemove);
                    Tournament.NumberOfPlayer = PlayersToTournament.ListPlayersToTournament.Count;
                    _tournamentsManager.UpdateTournament(Tournament);
                }
                else
                {
                    PlayersToTournament.RemovePlayerInTournament(playerToRemove);
                }
            }
        }
    }

    public abstract string ViewTournamentBracket();

    public abstract void StartTournament();

    public abstract void AddPlayers();

    public abstract void MovePlayer();

    public void ChangeRaceTo()
    {
        var duels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).ToList();
        var round = duels.First(d => d.EndGame == DateTime.MinValue).Round;

        if (duels.Any(p => p.Round == round && p.EndGame != DateTime.MinValue))
        {
            ConsoleService.WriteLineErrorMessage("Changing the number of games in this round is impossible because \n\r" +
                "the matches have already ended.");
            return;
        }
        else if (duels == null)
        {
            return;
        }
        ConsoleService.WriteTitle("Change Race To");
        var raceTo = ConsoleService.GetIntNumberFromUser("Enter To Many frame Min 3 Max 20:");
        if (raceTo == null || raceTo < 3 || raceTo > 20)
        {
            return;
        }

        if (!string.IsNullOrEmpty(round))
        {
            duels = duels.Where(d => d.Round == round).ToList();
        }

        foreach (var duel in duels)
        {
            duel.RaceTo = (int)raceTo;
            _singlePlayerDuelManager.UpdateSinglePlayerDuel(duel);
        }
    }
}