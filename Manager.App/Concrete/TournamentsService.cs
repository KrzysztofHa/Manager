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
    public void AddNewTurnament()
    {
        throw new NotImplementedException();
    }

    public void EndTurnament()
    {
        throw new NotImplementedException();
    }

    public void StartTurnament()
    {
        throw new NotImplementedException();
    }
}
