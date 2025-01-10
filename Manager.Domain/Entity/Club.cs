using Manager.Domain.Common;

namespace Manager.Domain.Entity;

public class Club : BaseEntity
{
    public string Name { get; set; }
    public int IdAddress { get; set; }
}