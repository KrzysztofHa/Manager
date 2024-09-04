namespace Manager.App.Abstract;

public interface IService<T>
{
    List<T> GetAllItem();

    int AddItem(T item);

    int UpdateItem(T item);

    void RemoveItem(T item);

    public T GetItemById(int id);

    void SaveList();
}