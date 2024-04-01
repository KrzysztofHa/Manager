using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Abstract
{
    public interface IService<T>
    {
        List<T> Items { get; set; }

        List<T> GetAllItem();
        int AddItem(T player);
        int UpdateItem(T player);
        void RemoveItem(T player);
    }
}
