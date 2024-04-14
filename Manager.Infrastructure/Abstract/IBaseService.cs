namespace Manager.Infrastructure.Abstract
{
    public interface IBaseService<T>
    {
        bool SaveListToBase(string PathName, List<T> listToSave);
        bool SaveOneRecordToBase(string PathName, T oneRecord);
        List<T> LoadListInBase(string PathName);
        void GetListInBaseOfPathNameView(string PathName);
    }
}
