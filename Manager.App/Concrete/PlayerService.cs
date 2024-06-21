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

            var findAddress = addressServis.GetAllItem().FirstOrDefault(a => a.Street == address.Street && a.BuildingNumber == address.BuildingNumber
                && a.City == address.City && a.Country == address.Country && a.Zip == address.Zip);

            if (findAddress != null)
            {
                findAddress.IsActive = true;
                player.IdAddress = findAddress.Id;
            }
            else
            {
                player.IdAddress = addressServis.AddItem(address);
                addressServis.SaveList();
            }
        }
        return player;
    }
    public Address GetPlayerAddress(Player player)
    {
        var address = new Address();
        IService<Address> addressServis = new BaseService<Address>();
        if (player != null)
        {
            address = addressServis.GetItemById(player.IdAddress);
        }

        return address;
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
            var formatAddressToView = string.Empty;
            if (playerAddressToView != null)
            {
                formatAddressToView = $" {playerAddressToView.Street,-10}".Remove(11) + $" {playerAddressToView.BuildingNumber,-10}".Remove(11) +
                 $" {playerAddressToView.City,-10}".Remove(11) + $" {playerAddressToView.Country,-10}".Remove(11) +
                    $" {playerAddressToView.Zip,-5}".Remove(6);
            }

            var formatPlayerDataToView = $"{player.Id,-5}".Remove(5) + $" {player.FirstName,-20}".Remove(21) +
                $" {player.LastName,-20}".Remove(21) + formatAddressToView;
            return $"{formatPlayerDataToView,-92}";
        }

        return string.Empty;
    }

    public List<Player> SearchPlayer(string searchString = " ")
    {
        List<Player> findPlayerList = new List<Player>();
        if (!string.IsNullOrEmpty(searchString))
        {
            findPlayerList = ListOfActivePlayers().Where(p => $"{p.Id} {p.FirstName} {p.LastName}".ToLower()
            .Contains(searchString.ToLower())).OrderBy(i => i.FirstName).ToList();
        }
        return findPlayerList;
    }

    public void DeletePlayer(Player player)
    {
        IService<Address> addressServis = new BaseService<Address>();

        var findPlayersWithTheSameIdAddress = GetAllItem().FindAll(p => p.IdAddress == player.IdAddress);

        if (findPlayersWithTheSameIdAddress.Count == 1)
        {
            addressServis.RemoveItem(addressServis.GetItemById(player.IdAddress));
            addressServis.SaveList();
        }
        RemoveItem(player);
        SaveList();

    }
}