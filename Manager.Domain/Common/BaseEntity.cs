namespace Manager.Domain.Common;

public class BaseEntity : AuditableModel
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}