using Manager.App.Abstract;
using Manager.App.Common;
using Manager.Domain.Entity;
using Manager.Infrastructure.Abstract;
using Manager.Infrastructure.Common;
using System;
using System.IO;


namespace Manager.App;

public class PlayerService: BaseService<Player>, IPlayerService, IService<Player>
{
    public List<Player> ListOfActivePlayers()
    {
        return GetAllItem().FindAll(p => p.IsActive == true);
    }

}