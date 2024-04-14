using Manager.App.Common;
using Manager.Domain.Entity;

namespace Manager.App
{
    public class PlayerService : BaseService<Player>
    {        
        public const string PathName = "Players";
        public List<Player> Players { get; set; }
        public PlayerService()
        {
            Players = new List<Player>();           
        }
    }
}