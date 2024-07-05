using Manager.App.Abstract;
using Manager.App.Common;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Concrete;

public class ClubService : BaseService<Club>, IClubService
{
    public List<Club> SearchClub(string searchString = " ")
    {
        List<Club> findClubsList = [];
        if (!string.IsNullOrEmpty(searchString))
        {
            findClubsList = [.. GetAllItem().Where(c => $"{c.Id} {c.Name}".ToLower()
            .Contains(searchString.ToLower())).OrderBy(i => i.Name)];
        }
        return findClubsList;
    }
    public Club AddClubAddress(Club club, Address address)
    {
        if (address != null && club != null)
        {
            IService<Address> addressServis = new BaseService<Address>();

            var findAddress = addressServis.GetAllItem().FirstOrDefault(a => a.Street == address.Street && a.BuildingNumber == address.BuildingNumber
                && a.City == address.City && a.Country == address.Country && a.Zip == address.Zip);

            if (findAddress != null)
            {
                findAddress.IsActive = true;
                club.IdAddress = findAddress.Id;
            }
            else
            {
                club.IdAddress = addressServis.AddItem(address);
                addressServis.SaveList();
            }
        }
        return club;
    }
    public string GetClubDetailView(Club club)
    {
        if (club != null)
        {
            IService<Address> clubAddress = new BaseService<Address>();
            var clubAddressToView = clubAddress.GetAllItem().FirstOrDefault(p => p.Id == club.IdAddress);
            var formatAddressToView = string.Empty;
            if (clubAddressToView != null)
            {
                formatAddressToView = $" {clubAddressToView.Street,-10}".Remove(11) + $" {clubAddressToView.BuildingNumber,-10}".Remove(11) +
                 $" {clubAddressToView.City,-10}".Remove(11) + $" {clubAddressToView.Country,-10}".Remove(11) +
                    $" {clubAddressToView.Zip,-5}".Remove(6);
            }

            var formatClubDataToView = $"{club.Id,-5}".Remove(5) + $" {club.Name,-20}".Remove(21) + formatAddressToView;
            return $"{formatClubDataToView,-72}";
        }

        return string.Empty;
    }
    public Club GetClubById(int idClub)
    {
        return GetItemById(idClub);
    }
}
