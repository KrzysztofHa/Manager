using Manager.App.Abstract;
using Manager.Domain.Common;
using Manager.Domain.Entity;
using Manager.Infrastructure.Abstract;
using Manager.Infrastructure.Common;

namespace Manager.App.Common;

public class BaseService<T> : IService<T> where T : BaseEntity
{
    private readonly IBaseService<T> _baseService = new BaseOperationService<T>();
    private List<T> Items { get; set; }

    public BaseService()
    {
        Items = new List<T>();
        LoadList();
    }
    public int AddItem(T item)
    {
        if (Items.Any())
        {
            item.Id = Items.Count + 1;
        }
        else
        {
            item.Id = 1;
        }
        item.IsActive = true;
        Items.Add(item);
        return item.Id;
    }

    public List<T> GetAllItem()
    {
        return Items;
    }

    public void RemoveItem(T item)
    {
        item.IsActive = false;
    }

    public int UpdateItem(T item)
    {
        var entity = Items.FirstOrDefault(p => p.Id == item.Id);
        if (entity != null)
        {
            entity = item;
        }
        return entity.Id;
    }
    public T GetItemOfId(int id)
    {
        var findItem = Items.FirstOrDefault(p => p.Id == id);
        return findItem;
    }
    public void LoadList() 
    {
        Items = _baseService.LoadListInBase();       
    }
    public void SaveList()
    {
        _baseService.SaveListToBase();
    }
}
