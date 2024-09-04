using Manager.Domain.Entity;

namespace Manager.App.Abstract;

public interface IClubManager
{
    Club SearchClub(string searchString);

    Club AddNewClub();

    void UpdateClub();
}