using System.ComponentModel.DataAnnotations;

namespace CleanArchitectureTemplate.Domain.SeedWork;

public abstract class BaseEntity<TKey> where TKey : struct, IComparable
{
    [Key]
    public virtual TKey Id { get; set; }

    protected BaseEntity() { }

    protected BaseEntity(TKey id)
    {
        Id = id;
    }
}
