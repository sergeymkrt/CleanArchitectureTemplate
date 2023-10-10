using System.ComponentModel.DataAnnotations;
using CleanArchitectureTemplate.Domain.Attributes;

namespace CleanArchitectureTemplate.Domain.SeedWork;

public abstract class LookupEntity : BaseEntity<int>
{
    [Required]
    [SearchColumn]
    public string Name { get; set; }

    protected LookupEntity() { }

    protected LookupEntity(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
