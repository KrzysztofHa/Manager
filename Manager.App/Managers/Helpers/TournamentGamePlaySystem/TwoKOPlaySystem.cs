using Manager.App.Abstract;
using Manager.App.Managers.Helpers.TournamentGamePlaySystem;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
        var newPlayers = PlayersToTournamentInPlaySystem.AddPlayersToTournament();

        if (newPlayers.Count > 0 && Tournament.Start != DateTime.MinValue)
        {
        }
    }

    protected override void MovePlayer()
    {
        List<Player> players = new List<Player>();
        foreach (var playerToTournament in PlayersToTournamentInPlaySystem.ListPlayersToTournament)
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

            var playerToMove = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == player.Id);

            if (playerToMove != null)
            {
                ConsoleService.WriteTitle("Move Player");
                ConsoleService.WriteLineMessage(ViewTournamentBracket());
                var newPosition = ConsoleService.GetIntNumberFromUser("Enter New Position", $"\n\r{PlayersToTournamentInPlaySystem.ViewPlayerToTournamentDetail(playerToMove)}");

                if (newPosition > 0 && newPosition != null && newPosition <= PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count)
                {
                    if (newPosition > playerToMove.Position)
                    {
                        for (int i = playerToMove.Position + 1; i <= (int)newPosition; i++)
                        {
                            var playerToChange = PlayersToTournamentInPlaySystem.ListPlayersToTournament
                            .First(p => p.Position == i);
                            playerToChange.Position = i - 1;

                            if (i == newPosition)
                            {
                                playerToMove.Position = (int)newPosition;
                            }
                        }
                    }
                    else if (newPosition < playerToMove.Position)
                    {
                        for (int i = playerToMove.Position - 1; i >= (int)newPosition; i--)
                        {
                            var playerToChange = PlayersToTournamentInPlaySystem.ListPlayersToTournament
                            .First(p => p.Position == i);

                            playerToChange.Position = i + 1;

                            if (i == newPosition)
                            {
                                playerToMove.Position = (int)newPosition;
                            }
                        }
                    }
                }
            }
        }
    }

    protected override void StartTournament()
    {
    }

    public override string ViewTournamentBracket()
    {
        var formatText = string.Empty;

        string lineOne = string.Empty;
        string lineTwo = string.Empty;
        int numberItemOfLine = 6;
        int item = 0;

        if (PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count > 0)
        {
            formatText += $"\n\rStart List 2KO System\n\r\n\r";
            foreach (var player in PlayersToTournamentInPlaySystem.ListPlayersToTournament.OrderBy(p => p.Position))
            {
                if (player.Position % 2 != 0)
                {
                    lineOne += $" {player.Position}. {player.TinyFulName}".Remove(20);
                }
                else
                {
                    lineTwo += $" {player.Position}. {player.TinyFulName}".Remove(20);
                }

                item++;

                if (item == numberItemOfLine * 2 ||
                    player.Position == PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count)
                {
                    if (player.Position % 2 != 0)
                    {
                        lineTwo += $"{" Free Win ",-30}";
                    }
                    formatText += lineOne + "\n\r" + lineTwo + "\n\r\n\r";
                    lineOne = string.Empty;
                    lineTwo = string.Empty;
                    item = 0;
                }
            }
        }

        return formatText;
    }

    protected override void ExecuteExtendedAction(MenuAction menuAction)
    {
        var swichOption = menuAction.Id;
        switch (swichOption)
        {
            case 1:
                if (Tournament.Start == DateTime.MinValue)
                {
                    ConsoleService.WriteTitle($"Start Tournament {Tournament.Name}");
                    if (ConsoleService.AnswerYesOrNo("Before you proceed, make sure is correct everything."))
                    {
                        StartTournament();
                    }
                }
                break;

            default:
                ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
                break;
        }
    }

    protected override List<MenuAction> GetExtendedMenuAction()
    {
        List<MenuAction> actions =
        [
               new MenuAction(1, "  <-----  Start Tournament", "GroupPlaySystem")
        ];

        if (Tournament.NumberOfPlayer < 8 && actions.Exists(a => a.Name == "  <-----  Start Tournament"))
        {
            actions.Remove(actions.First(a => a.Name == "  <-----  Start Tournament"));
        }

        return actions;
    }

    protected override void RemovePlayers(PlayerToTournament playerToRemove)
    {
        if (playerToRemove != null)
        {
            _singlePlayerDuelManager.RemoveTournamentDuel(Tournament, playerToRemove.IdPLayer);
        }
    }
}