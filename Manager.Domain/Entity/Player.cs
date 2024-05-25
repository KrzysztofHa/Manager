using Manager.Domain.Common;

namespace Manager.Domain.Entity;

public class Player : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; } 
    public string Country { get; set; }
    public string City { get; set; }
}
