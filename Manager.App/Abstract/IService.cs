namespace Manager.App.Abstract;

public interface IService<T>
{
    List<T> GetAllItem();

    int AddItem(T item);

    void AddRangeItems(List<T> items);

    int UpdateItem(T item);

    void RemoveItem(T item);

    public T GetItemById(int id);

    void SaveList();
}