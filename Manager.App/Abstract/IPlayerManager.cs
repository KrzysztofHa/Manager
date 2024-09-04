using Manager.Domain.Entity;

namespace Manager.App.Abstract
{
    public interface IPlayerManager
    {
        Player AddNewPlayer();

        Player UpdatePlayer();

        Player SearchPlayer(string title = "", List<Player> playersList = null);

        void PlayerOptionView();
    }
}