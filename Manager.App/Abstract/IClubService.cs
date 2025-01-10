using Manager.Domain.Entity;

namespace Manager.App.Abstract
{
    public interface IClubService : IService<Club>
    {
        List<Club> SearchClub(string searchString);

        public Club AddClubAddress(Club club, Address address);

        public string GetClubDetailView(Club club);

        Club GetClubById(int idClub);
    }
}