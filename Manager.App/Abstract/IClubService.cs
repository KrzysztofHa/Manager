using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Abstract
{
    public interface IClubService : IService<Club>
    {
        List<Club> SearchClub(string searchString);
        public Club AddClubAddress(Club club, Address address);
        public string GetClubDetailView(Club club);
    }
}
