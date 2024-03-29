using Manager.App.Common;
using Manager.Domain.Entity;

namespace Manager.App
{
    public class PleyerService : BaseService<Pleyer>
    {
        public List<Pleyer> Pleyers { get; set; }
        public PleyerService()
        {
            Pleyers = new List<Pleyer>();
        }
    }
}