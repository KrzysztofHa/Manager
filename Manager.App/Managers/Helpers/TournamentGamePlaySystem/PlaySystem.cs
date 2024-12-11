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

public abstract class PlaySystem
{
    public Tournament Tournament { get; }

    protected PlaySystem(Tournament tournament)
    {
        Tournament = tournament;
    }

    public abstract string ViewTournamentBracket(PlayersToTournament playersToTournament);

    public abstract void StartTournament();
}