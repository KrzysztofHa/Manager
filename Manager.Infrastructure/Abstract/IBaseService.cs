using Manager.Infrastructure.Concrete;

namespace Manager.Infrastructure.Abstract
{
    public interface IBaseService<T>
    {
        List<T> ListOfElements { get; set; }
        bool SaveListToBase();
        List<T> LoadListInBase();
    }
}
