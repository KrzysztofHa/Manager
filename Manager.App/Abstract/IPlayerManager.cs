using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
