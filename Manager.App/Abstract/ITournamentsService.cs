using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Abstract
{
    public interface ITournamentsService : IService<Tournament>
    {
        void AddNewTournament(Tournament tournament);
        void StartTournament(Tournament tournament);
        void EndTournament(Tournament tournament);
        List<Tournament> SearchTournament(string searchString);
        string GetTournamentDetailView(Tournament tournament);
    }
}