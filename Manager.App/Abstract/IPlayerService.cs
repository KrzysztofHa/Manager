using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Abstract;

public interface IPlayerService : IService<Player>
{
    public List<Player> ListOfActivePlayers();
    public List<Player> SearchPlayer(string searchString);
    public Player AddPlayerAddress(Player player, Address address);
    public string GetPlayerDetailView(Player player);
}
