using Manager.Infrastructure.Abstract;

namespace Manager.Infrastructure.Common
{
    public class BaseOperationService<T> : IBaseService<T>
    {
        public void GetListInBaseOfPathNameView(string PathName)
        {
            throw new NotImplementedException();
        }

        public List<T> LoadListInBase(string PathName)
        {
            throw new NotImplementedException();
        }

        public bool SaveListToBase(string PathName, List<T> listToSave)
        {
            throw new NotImplementedException();
        }

        public bool SaveOneRecordToBase(string PathName, T oneRecord)
        {
            throw new NotImplementedException();
        }

    }

}


