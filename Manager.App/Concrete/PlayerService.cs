using Manager.App.Abstract;
using Manager.App.Common;
using Manager.Domain.Entity;
using Manager.Infrastructure.Abstract;
using Manager.Infrastructure.Common;
using System;
using System.IO;
using System.Net;
using Xunit.Abstractions;


namespace Manager.App;

public class PlayerService : BaseService<Player>, IPlayerService, IService<Player>
{
    public Player AddPlayerAddress(Player player, Address address)
    {
        if (address != null && player != null)
        {
            IService<Address> addressServis = new BaseService<Address>();
            player.IdAddress = addressServis.AddItem(address);
            addressServis.SaveList();
        }
        return player;
    }

    public List<Player> ListOfActivePlayers()
    {
        return GetAllItem().FindAll(p => p.IsActive == true);
    }

    public string GetPlayerDetailView(Player player)
    {
        if (player != null)
        {

            IService<Address> playerAddress = new BaseService<Address>();
            var playerAddressToView = playerAddress.GetAllItem().FirstOrDefault(p => p.Id == player.IdAddress);
            var viewAddress = string.Empty;
            if (playerAddressToView != null)
            {
                viewAddress = $"{playerAddressToView.Street,-1}{playerAddressToView.BuildingNumber,-5}" +
                 $"{playerAddressToView.City,-10}{playerAddressToView.Country,-10}{playerAddressToView.zip,-10}";
            }

            return $"{player.Id,-5}{player.FirstName,-10}{player.LastName,-10}" + viewAddress;
        }

        return string.Empty;
    }

    public List<Player> SearchPlayer(string searchString)
    {
        List<Player> findPlayerList = new List<Player>();
        if (!string.IsNullOrEmpty(searchString))
        {
            IService<Address> addressServis = new BaseService<Address>();
            findPlayerList = ListOfActivePlayers().Where(p => $"{p.Id} {p.FirstName} {p.LastName}".ToLower()
            .Contains(searchString.ToLower())).OrderBy(i => i.FirstName).ToList();
        }
        return findPlayerList;
    }
}