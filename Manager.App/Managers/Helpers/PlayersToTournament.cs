using Manager.App.Abstract;
using Manager.Domain.Entity;
using Manager.Infrastructure.Abstract;
using Manager.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Managers.Helpers;

public class PlayersToTournament
{
    public List<PlayerToTournament> ListPlayersToTournament { get; set; }
    public int? IdTournament { get; set; }
    public PlayersToTournament()
    {
    }
    public PlayersToTournament(Tournament tournament)
    {
        ListPlayersToTournament = new List<PlayerToTournament>();
        IdTournament = tournament.Id;
        LoadList();
    }
    private void LoadList()
    {
        IBaseService<PlayersToTournament> baseService = new BaseOperationService<PlayersToTournament>();
        if (baseService.ListOfElements.Count == 0 ||
            !baseService.ListOfElements.Any(l => l == this))
        {
            baseService.ListOfElements.Add(this);
            baseService.SaveListToBase();
        }

        var checkList = baseService.ListOfElements
       .Where(l => l.IdTournament == IdTournament).Select(e => e.GetPlayerToTournament()).First();
        ListPlayersToTournament = checkList;
    }

    public void SavePlayersToTournament()
    {
        IBaseService<PlayersToTournament> baseService = new BaseOperationService<PlayersToTournament>();
        var listPlayer = baseService.ListOfElements.FirstOrDefault(this);
        listPlayer.ListPlayersToTournament = ListPlayersToTournament;
        baseService.SaveListToBase();
    }
    public List<PlayerToTournament> GetPlayerToTournament()
    {
        return ListPlayersToTournament;
    }

}
