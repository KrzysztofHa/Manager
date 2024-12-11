using Manager.App.Abstract;
using Manager.Domain.Entity;
using Manager.Infrastructure.Abstract;
using Manager.Infrastructure.Common;

namespace Manager.App.Managers.Helpers;

public class PlayersToTournament
{
    private readonly ITournamentsManager _tournamentsManager;
    public List<PlayerToTournament> ListPlayersToTournament { get; set; }
    public int IdTournament { get; set; }

    public PlayersToTournament()
    {
    }

    public PlayersToTournament(Tournament tournament, ITournamentsManager tournamentsManager)
    {
        if (tournament == null)
        {
            return;
        }
        _tournamentsManager = tournamentsManager;
        ListPlayersToTournament = new List<PlayerToTournament>();
        IdTournament = tournament.Id;
        LoadList(tournament);
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
}