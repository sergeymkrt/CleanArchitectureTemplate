using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitectureTemplate.Domain.SeedWork;

public abstract class RegularEntity : BaseEntity<long>
{
    [NotMapped]
    public virtual string DisplayName => Id.ToString();

    private List<DomainEvent> _domainEvents;
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

    public void AddDomainEvent(DomainEvent eventItem)
    {
        _domainEvents ??= new List<DomainEvent>();
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(DomainEvent eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    public bool IsTransient()
    {
        return Id == default;
    }

    public override bool Equals(object obj)
    {
        if (obj is not RegularEntity item)
            return false;

        if (ReferenceEquals(this, item))
            return true;

        if (GetType() != item.GetType())
            return false;

        if (item.IsTransient() || IsTransient())
            return false;
        return item.Id == Id;
    }

    public static bool operator ==(RegularEntity left, RegularEntity right)
    {
        return Equals(left, null) ? Equals(right, null) : left.Equals(right);
    }

    public static bool operator !=(RegularEntity left, RegularEntity right)
    {
        return !(left == right);
    }
}
