namespace Manager.App.Abstract
{
    public interface IService<T>
    {
        //List<T> Items { get; set; }
        List<T> GetAllItem();
        int AddItem(T item);
        int UpdateItem(T item);
        void RemoveItem(T item);
        public T GetItemOfId(int id);
        void SaveList();
    }
}
