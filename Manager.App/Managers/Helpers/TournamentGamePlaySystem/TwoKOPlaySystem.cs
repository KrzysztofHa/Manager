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

    protected override void MovePlayer()
    {
        List<Player> players = new List<Player>();
        foreach (var playerToTournament in PlayersToTournament.ListPlayersToTournament)
        {
            var player = _playerService.GetItemById(playerToTournament.IdPLayer);
            bool isPlayerEndDuelOrPlay = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
               .Exists(p => (p.IdFirstPlayer == player.Id || p.IdSecondPlayer == player.Id) && (p.StartGame != DateTime.MinValue && p.Interrupted == DateTime.MinValue || p.EndGame != DateTime.MinValue));
            if (player != null && !isPlayerEndDuelOrPlay)
            {
                players.Add(player);
            }
        }

        if (players.Count > 0)
        {
            var player = _playerManager.SearchPlayer("Select Player To Move\n\r" +
                "List of players who can currently be transferred", players);

            if (player == null)
            {
                return;
            }

            var playerToMove = PlayersToTournament.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == player.Id);

            if (playerToMove != null)
            {
                ConsoleService.WriteTitle("Move Player");
                ConsoleService.WriteLineMessage(ViewTournamentBracket());

                if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id) != null)
                {
                    ConsoleService.WriteLineErrorMessage("Transferring a player is impossible");
                    return;
                }
                ConsoleService.WriteTitle("Move Player");
                ConsoleService.WriteLineMessage(ViewTournamentBracket());
                var newPosition = ConsoleService.GetIntNumberFromUser("Enter New Position", $"\n\r{PlayersToTournament.ViewPlayerToTournamentDetail(playerToMove)}");

                if (newPosition > 0 && newPosition != null && newPosition <= PlayersToTournament.ListPlayersToTournament.Count)
                {
                    if (newPosition > playerToMove.Position)
                    {
                        for (int i = playerToMove.Position + 1; i <= (int)newPosition; i++)
                        {
                            var playerToChange = PlayersToTournament.ListPlayersToTournament
                            .First(p => p.Position == i);
                            playerToChange.Position = i - 1;
                            playerToChange.TwoKO = playerToChange.Position.ToString();

                            if (i == newPosition)
                            {
                                playerToMove.Position = (int)newPosition;
                                playerToMove.TwoKO = playerToMove.Position.ToString();
                            }
                        }
                    }
                    else if (newPosition < playerToMove.Position)
                    {
                        for (int i = playerToMove.Position - 1; i >= (int)newPosition; i--)
                        {
                            var playerToChange = PlayersToTournament.ListPlayersToTournament
                            .First(p => p.Position == i);

                            playerToChange.Position = i + 1;
                            playerToChange.TwoKO = playerToChange.Position.ToString();

                            if (i == newPosition)
                            {
                                playerToMove.Position = (int)newPosition;
                                playerToMove.TwoKO = playerToMove.Position.ToString();
                            }
                        }
                    }
                }
            }
            PlayersToTournament.SavePlayersToTournament();
        }
    }

    protected override void StartTournament()
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

    protected override void ExecuteExtendedAction(MenuAction menuAction)
    {
        throw new NotImplementedException();
    }

    protected override List<MenuAction> GetExtendedMenuAction()
    {
        return new List<MenuAction>();
    }

    protected override void RemovePlayers(PlayerToTournament playerToRemove)
    {
        if (playerToRemove != null)
        {
            _singlePlayerDuelManager.RemoveTournamentDuel(Tournament, playerToRemove.IdPLayer);
        }
    }
}