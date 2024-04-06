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
            }
            else
            {
                return 1;
            }
        }

        public int AddItem(T item)
        {
            Items.Add(item);
            return item.Id;
        }

        public List<T> GetAllItem()
        {
            return Items;
        }

        public void RemoveItem(T item)
        {
            Items.Remove(item);
        }

        public int UpdateItem(T item)
        {
            var entity = Items.FirstOrDefault(p => p.Id == item.Id);
            if (entity != null)
            {
                entity = item;
            }
            return entity.Id;
        }
    }
}
