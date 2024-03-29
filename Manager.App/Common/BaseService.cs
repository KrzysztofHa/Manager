using Manager.App.Abstract;
using Manager.Domain.Common;

namespace Manager.App.Common
{
    public class BaseService<T> : IService<T> where T : BaseEntity
    {
        public List<T> SomeItem { get; set; }

        public BaseService()
        {
            SomeItem = new List<T>();
        }

        public int GetNextId() 
        {
            if (SomeItem.Any())
            {
                return SomeItem.Count() + 1;
            }else
            {
                return 1;
            }                       
        }

        public int AddSomeItem(T pleyer)
        {
            SomeItem.Add(pleyer);
            return pleyer.Id;
        }

        public List<T> GetAllSomeItem()
        {
            return SomeItem;
        }

        public void RemoveSomeItem(T pleyer)
        {
            SomeItem.Remove(pleyer);
        }

        public int UpdateSomeItem(T pleyer)
        {
            var entity = SomeItem.FirstOrDefault(p => p.Id == pleyer.Id);
            if (entity != null)
            {
                entity = pleyer;
            }
            return entity.Id;
        }
    }
}
