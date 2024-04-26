using Manager.App.Common;
using Manager.Domain.Entity;
using Manager.Infrastructure.Abstract;
using Manager.Infrastructure.Common;

namespace Manager.App
{
    public class PlayerService : BaseService<Player>
    {
        public List<Player> Players { get; set; }

        public PlayerService()
        {
            Players = new List<Player>();
        }     
    }
}