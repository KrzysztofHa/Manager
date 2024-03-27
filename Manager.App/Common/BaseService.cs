using Manager.App.Abstract;
using Manager.Domain.Common;

namespace Manager.App.Common
{
    public class BaseService<T> : IService<T> where T : BaseEntity
    {
        public List<T> Pleyers { get; set; }
        public BaseService()
        {
            Pleyers = new List<T>();
        }
        public int AddPleyer(T pleyer)
        {
            Pleyers.Add(pleyer);
            return pleyer.Id;
        }

        public List<T> GetAllPleyers()
        {
            return Pleyers;
        }

        public void RemovePleyer(T pleyer)
        {
            Pleyers.Remove(pleyer);
        }

        public int UpdatePleyer(T pleyer)
        {
            var entity = Pleyers.FirstOrDefault(p => p.Id == pleyer.Id);
            if (entity != null)
            {
                entity = pleyer;
            }
            return entity.Id;
        }
    }
}
