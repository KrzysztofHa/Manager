using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Abstract
{
    public interface IService<T>
    {
        List<T> SomeItem { get; set; }

        List<T> GetAllSomeItem();
        int AddSomeItem(T pleyer);
        int UpdateSomeItem(T pleyer);
        void RemoveSomeItem(T pleyer);
    }
}
