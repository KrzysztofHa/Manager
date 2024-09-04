using Manager.Domain.Common;

namespace Manager.Domain.Entity;

public class User : BaseEntity
{
    public string UserName { get; set; }
    public int? PlayerId { get; set; }
    public string DisplayName { get; set; }
}