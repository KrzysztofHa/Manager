using Manager.Domain.Common;

namespace Manager.Domain.Entity;

public class Tournament : BaseEntity
{
    public string Name { get; set; }
    public int IdClub { get; set; }
    public int IdLeague { get; set; }
}
