using Manager.App.Abstract;
using Manager.Domain.Common;

namespace Manager.App.Common
{
    public class BaseService<T> : IService<T> where T : BaseEntity
    {
        public List<T> Items { get; set; }

        public BaseService()
        {
            Items = new List<T>();
        }

        public int GetNextId() 
        {
            if (Items.Any())
            {
                return Items.Count() + 1;
            }else
            {
                return 1;
            }                       
        }

        public int AddItem(T player)
        {
            Items.Add(player);
            return player.Id;
        }

        public List<T> GetAllItem()
        {
            return Items;
        }

        public void RemoveItem(T player)
        {
            Items.Remove(player);
        }

        public int UpdateItem(T player)
        {
            var entity = Items.FirstOrDefault(p => p.Id == player.Id);
            if (entity != null)
            {
                entity = player;
            }
            return entity.Id;
        }
    }
}
