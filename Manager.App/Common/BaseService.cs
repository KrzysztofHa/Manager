using Manager.App.Abstract;
using Manager.Domain.Common;
using Manager.Helpers;
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
        item.CreatedById = ActiveUserNameOrId.IdActiveUser;
        item.IsActive = true;
        item.CreatedDateTime = DateTime.Now;
        Items.Add(item);

        return item.Id;
    }

    public List<T> GetAllItem()
    {
        return Items;
    }

    public void RemoveItem(T item)
    {
        if (Items.Any() && item != null)
        {
            item.ModifiedDateTime = DateTime.Now;
            item.ModifiedDateTime = DateTime.Now;
            item.IsActive = false;
        }
    }

    public int UpdateItem(T item)
    {
        var entity = Items.FirstOrDefault(p => p.Id == item.Id);
        if (entity != null)
        {
            entity = item;
            entity.ModifiedDateTime = DateTime.Now;
            entity.ModifiedById = ActiveUserNameOrId.IdActiveUser;
        }
        return entity.Id;
    }

    public T GetItemById(int id)
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