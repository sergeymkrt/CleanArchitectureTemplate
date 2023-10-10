using CleanArchitectureTemplate.Domain.SeedWork;

namespace CleanArchitectureTemplate.Domain.Shared.Lookups;

public class RoundingRule : LookupEntity
{
    public RoundingRule()
    {

    }

    public RoundingRule(int id, string name) : base(id, name)
    {

    }
}
