using Manager.App.Abstract;
using Manager.Domain.Common;
using Manager.Domain.Entity;
using Manager.Infrastructure.Abstract;
using Manager.Infrastructure.Common;

namespace Manager.App.Common
{
    public class BaseService<T> : IService<T> where T : BaseEntity
    {       
        public List<T> Items { get; set; }

        public BaseService()
        {
            Items = new List<T>();           
        }
        public int AddItem(T item)
        {           
            Items.Add(item);
            return item.Id;
        }

        public List<T> GetAllItem()
        {
            return Items.FindAll(p => p.Active == true);
        }

        public void RemoveItem(T item)
        {
            item.Active = false;
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
