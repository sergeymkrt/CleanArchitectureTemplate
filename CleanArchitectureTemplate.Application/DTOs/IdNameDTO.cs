namespace CleanArchitectureTemplate.Application.DTOs;

public class IdNameDto
{
    public long Id { get; set; }
    public string Name { get; set; }

    public IdNameDto() { }

    public IdNameDto(long id, string name)
    {
        Id = id;
        Name = name;
    }
}
