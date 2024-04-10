using Manager.Infrastructure.Common;

namespace Manager.Infrastructure.Abstract
{
    public interface IBaseService<T>
    {
        List<BasePaths> Paths { get; set; }
        bool Save(string PathName, List<T> listToFile);
        List<T> Load(string fileName);
        List<BasePaths> BasePathsList();
    }
}
