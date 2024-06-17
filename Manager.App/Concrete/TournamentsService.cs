using Manager.App.Abstract;
using Manager.App.Common;
using Manager.App.Managers;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Concrete;

public class TournamentsService : BaseService<Tournament>, ITournamentsService
{
    public void AddNewTournament(Tournament tournament)
    {
        AddItem(tournament);
        SaveList();
    }

    public void EndTournament(Tournament tournament)
    {
        throw new NotImplementedException();
    }

    public void StartTournament(Tournament tournament)
    {
        throw new NotImplementedException();
    }
}
