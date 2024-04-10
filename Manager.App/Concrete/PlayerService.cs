using Manager.App.Common;
using Manager.Domain.Entity;

namespace Manager.App
{
    public class PlayerService : BaseService<Player>
    {
        // private readonly ISaveItem<Path> _SaveItem;
        public const string PathName = "Players";
        public List<Player> Players { get; set; }
        public PlayerService()//ISaveItem<Path> SaveItem)
        {
            Players = new List<Player>();
            //_SaveItem = SaveItem;
        }
    }
}