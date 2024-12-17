using Manager.App.Abstract;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using Manager.Infrastructure.Abstract;
using Manager.Infrastructure.Common;

namespace Manager.App.Managers.Helpers;

public class PlayersToTournament
{
    private readonly ITournamentsManager _tournamentsManager;
    private readonly IPlayerManager _playerManager;
    private readonly IPlayerService _playerService;
    public List<PlayerToTournament> ListPlayersToTournament { get; set; }
    public int IdTournament { get; set; }
    private Tournament _tournament;

    public PlayersToTournament()
    {
    }

    public PlayersToTournament(Tournament tournament, ITournamentsManager tournamentsManager, IPlayerManager playerManager, IPlayerService playerService)
    {
        if (tournament == null)
        {
            return;
        }
        _tournamentsManager = tournamentsManager;
        ListPlayersToTournament = new List<PlayerToTournament>();
        IdTournament = tournament.Id;
        LoadList(tournament);
        _tournament = tournament;
        _playerManager = playerManager;
        _playerService = playerService;
    }

    public void LoadList(Tournament tournament)
    {
        IBaseService<PlayersToTournament> baseService = new BaseOperationService<PlayersToTournament>();
        if (baseService.ListOfElements.Count == 0 ||
            !baseService.ListOfElements.Any(l => l.IdTournament == this.IdTournament))
        {
            baseService.ListOfElements.Add(this);
            baseService.SaveListToBase();
        }

        var checkList = baseService.ListOfElements
       .Where(l => l.IdTournament == IdTournament).Select(e => e.GetPlayerToTournament()).First();
        if (checkList.Count != tournament.NumberOfPlayer)
        {
            tournament.NumberOfPlayer = checkList.Count;
            _tournamentsManager.UpdateTournament(tournament);
        }
        ListPlayersToTournament = checkList;
    }

    public void SavePlayersToTournament()
    {
        IBaseService<PlayersToTournament> baseService = new BaseOperationService<PlayersToTournament>();
        var listPlayer = baseService.ListOfElements.FirstOrDefault(p => p.IdTournament == this.IdTournament);
        listPlayer.ListPlayersToTournament = ListPlayersToTournament;
        baseService.SaveListToBase();
    }

    public List<PlayerToTournament> GetPlayerToTournament()
    {
        return ListPlayersToTournament;
    }

    public void AddPlayersToTournament()
    {
        List<Player> players = new List<Player>();

        foreach (var playerToTournament in ListPlayersToTournament)
        {
            var tournamentPlayer = _playerService.GetItemById(playerToTournament.IdPLayer);
            if (tournamentPlayer != null)
            {
                players.Add(tournamentPlayer);
            }
        }

        var player = _playerManager.SearchPlayer($"Add Player To tournament {_tournament.Name}" +
            $"\n\rSelect Player On List Or Press Esc To Add New Player",
            null,
            players);

        if (player == null)
        {
            if (ConsoleService.AnswerYesOrNo("You want to add a new player"))
            {
                player = _playerManager.AddNewPlayer();
            }

            if (player == null)
            {
                return;
            }
        }
        var playeraddress = _playerService.GetPlayerAddress(player);

        if (ListPlayersToTournament.Any(p => p.IdPLayer == player.Id))
        {
            ConsoleService.WriteLineErrorMessage($"The Player {player.FirstName} {player.LastName} is on the list");
        }
        else
        {
            PlayerToTournament newPlayer = new(player, "------");

            if (ListPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.Group)))
            {
                var groupingPlayers = ListPlayersToTournament
               .GroupBy(group => group.Group, group => group).OrderBy(g => g.Count());
                newPlayer.Position = ListPlayersToTournament.Max(p => p.Position) + 1;
                newPlayer.TwoKO = newPlayer.Position.ToString();
                newPlayer.Group = groupingPlayers.First().Key;
            }
            else
            {
                if (ListPlayersToTournament.Any())
                {
                    newPlayer.Position = ListPlayersToTournament.Max(p => p.Position) + 1;
                    newPlayer.TwoKO = newPlayer.Position.ToString();
                }
                else
                {
                    newPlayer.Position = 1;
                    newPlayer.TwoKO = newPlayer.Position.ToString();
                }
            }

            if (playeraddress != null)
            {
                newPlayer.Country = playeraddress.Country;
            }

            ListPlayersToTournament.Add(newPlayer);
            _tournament.NumberOfPlayer = ListPlayersToTournament.Count;
        }
        _tournamentsManager.UpdateTournament(_tournament);
        SavePlayersToTournament();

        ConsoleService.WriteTitle("");
        if (ConsoleService.AnswerYesOrNo("Add Next Player"))
        {
            AddPlayersToTournament();
        }
    }

    public void RemovePlayerInTournament()
    {
        List<Player> players = new List<Player>();
        if (PLayersList.Count > 8)
        {
            ConsoleService.WriteTitle("");
            ConsoleService.WriteLineErrorMessage("Attention!!!");
            if (!ConsoleService.AnswerYesOrNo("Remember that removing a player may disturb the group structure.\n\r" +
                "Players who are currently playing or have finished the match will not appear on the list."))
            {
                return;
            }

            foreach (var playerToTournament in PLayersList)
            {
                var player = _playerService.GetItemById(playerToTournament.IdPLayer);
                bool isPlayerEndDuelOrPlay = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
               .Exists(p => (p.IdFirstPlayer == player.Id || p.IdSecondPlayer == player.Id) && (p.StartGame != DateTime.MinValue && p.Interrupted == DateTime.MinValue) || p.EndGame != DateTime.MinValue);

                if (player != null && !isPlayerEndDuelOrPlay)
                {
                    players.Add(player);
                }
            }
        }
        else
        {
            ConsoleService.WriteLineErrorMessage("You cannot remove a player. \n\rThe minimum number of players is 8.");
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
                var playerToRemove = PLayersList.FirstOrDefault(p => p.IdPLayer == player.Id);

                if (playerToRemove != null)
                {
                    if (Tournament.GamePlaySystem == "Group")
                    {
                        var groupingPlayer = PLayersList.GroupBy(d => d.Group);
                        if (groupingPlayer.FirstOrDefault(g => g.Key == playerToRemove.Group).Count() <= 2)
                        {
                            ConsoleService.WriteLineErrorMessage("You cannot remove a player. Minimum number of players in group 2.");
                            return;
                        }
                        else
                        {
                            _singlePlayerDuelManager.RemoveTournamentDuel(Tournament, playerToRemove.IdPLayer);
                            PLayersList.Remove(playerToRemove);
                            playersToTournament.SavePlayersToTournament();
                            Tournament.NumberOfPlayer = PLayersList.Count;
                            _tournamentsManager.UpdateTournament(Tournament);
                        }
                    }
                    else
                    {
                        _singlePlayerDuelManager.RemoveTournamentDuel(Tournament, playerToRemove.IdPLayer);
                        PLayersList.Remove(playerToRemove);
                        playersToTournament.SavePlayersToTournament();
                        Tournament.NumberOfPlayer = PLayersList.Count;
                        _tournamentsManager.UpdateTournament(Tournament);
                    }
                }
            }
        }
    }
}