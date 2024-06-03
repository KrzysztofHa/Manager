using Manager.App.Abstract;
using Manager.App.Common;
using Manager.Domain.Entity;
using Manager.Infrastructure.Abstract;
using Manager.Infrastructure.Common;
using System;
using System.IO;
using Xunit.Abstractions;


namespace Manager.App;

public class PlayerService : BaseService<Player>, IPlayerService, IService<Player>
{
    public List<Player> ListOfActivePlayers()
    {
        return GetAllItem().FindAll(p => p.IsActive == true);
    }
    public List<Player> SearchPlayer(string searchString)
    {
        var findPlayerList = ListOfActivePlayers().Where(p => $"{p.Id} {p.FirstName} {p.LastName} {p.City} {p.Country}".ToLower()
        .Contains(searchString.ToLower())).OrderBy(i => i.FirstName).ToList();
        
        return findPlayerList;
    }
}