using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Abstract
{
    public interface IService<T>
    {
        List<T> Pleyers { get; set; }

        List<T> GetAllPleyers();
        int AddPleyer(T pleyer);
        int UpdatePleyer(T pleyer);
        void RemovePleyer(T pleyer);
    }
}
