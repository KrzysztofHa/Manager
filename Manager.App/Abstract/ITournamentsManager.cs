using Manager.App.Managers.Helpers;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Abstract
{
    public interface ITournamentsManager
    {
        void AddPlayersToTournament(Tournament tournament, PlayersToTournament playersToTournament);

        void ViewListPlayersToTournament(Tournament tournament, PlayersToTournament playersToTournament);

        string ViewPlayerToTournamentDetail(PlayerToTournament playerToTournament);

        void RemovePlayerOfTournament(Tournament tournament, PlayersToTournament playersToTournament);

        //void ChangeNumberOfTable(Tournament tournament, PlayersToTournament playersToTournament);
    }
}