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
       .Where(l => l.IdTournament == IdTournament).Select(e => e.GetPlayersToTournament()).First();
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
        listPlayer.ListPlayersToTournament = ListPlayersToTournament.OrderBy(p => p.Position).ToList();
        baseService.SaveListToBase();
    }

    public List<PlayerToTournament> GetPlayersToTournament()
    {
        return ListPlayersToTournament;
    }

    public List<PlayerToTournament> AddPlayersToTournament()
    {
        List<Player> players = new List<Player>();
        List<PlayerToTournament> newPlayersList = new List<PlayerToTournament>();

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
                return newPlayersList;
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

            if (playeraddress != null)
            {
                newPlayer.Country = playeraddress.Country;
            }

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

            ListPlayersToTournament.Add(newPlayer);
            newPlayersList.Add(newPlayer);
            _tournament.NumberOfPlayer = ListPlayersToTournament.Count;
        }
        _tournamentsManager.UpdateTournament(_tournament);
        SavePlayersToTournament();
        ViewListPlayersToTournament();
        if (ConsoleService.AnswerYesOrNo("Add Next Player"))
        {
            newPlayersList.AddRange(AddPlayersToTournament());
        }
        return newPlayersList;
    }

    public void RemovePlayerInTournament(PlayerToTournament playerToRemove)
    {
        if (playerToRemove != null)
        {
            ListPlayersToTournament.Remove(playerToRemove);
            for (int i = 0; i < ListPlayersToTournament.Count; i++)
            {
                ListPlayersToTournament[i].Position = i + 1;
            }

            SavePlayersToTournament();
        }
    }

    public void ViewListPlayersToTournament()
    {
        string formatText = string.Empty;
        if (ListPlayersToTournament.Any())
        {
            ConsoleService.WriteTitle($"List Players Of {_tournament.Name}");
            foreach (var player in ListPlayersToTournament)
            {
                formatText = $"{player.Position}. {player.TinyFulName} {player.Country}";
                ConsoleService.WriteLineMessage(formatText);
            }
        }
        else
        {
            ConsoleService.WriteTitle($"List Players Of {_tournament.Name}");
            ConsoleService.WriteLineErrorMessage("Empty List");
        }
    }

    public string ViewPlayerToTournamentDetail(PlayerToTournament playerToTournament)
    {
        string formatText = string.Empty;
        if (playerToTournament != null)
        {
            formatText = $"Position: {playerToTournament.Position}.  {playerToTournament.TinyFulName} ";
        }
        return formatText;
    }
}