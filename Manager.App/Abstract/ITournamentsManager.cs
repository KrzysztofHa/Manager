using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Abstract;

public interface ITournamentsManager
{
    public void UpdateTournament(Tournament tournament);

    void InterruptTournament(Tournament tournament);

    string GetTournamentDetailView(Tournament tournament);

    void EndTournament(Tournament tournament);

    void StartTournament(Tournament tournament);
}