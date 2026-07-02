namespace Domain.Models.Interfaces
{
    public interface IAuditableEntity
    {
        Guid? CreatedBy { get; set; }
        DateTime CreatedAt { get; set; }
        Guid? LastModifiedBy { get; set; }
        DateTime? LastModifiedAt { get; set; }
        Guid? DeletedBy { get; set; }
        DateTime? DeletedAt { get; set; }
        bool IsDeleted { get; set; }
    }
}
