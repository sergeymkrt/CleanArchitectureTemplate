namespace CleanArchitectureTemplate.Domain.Models;

public class IdName
{
    public long Id { get; set; }
    public string Name { get; set; }

    public IdName() { }

    public IdName(long id, string name)
    {
        Id = id;
        Name = name;
    }
}
