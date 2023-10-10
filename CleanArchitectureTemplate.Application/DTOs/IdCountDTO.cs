namespace CleanArchitectureTemplate.Application.DTOs;

public class IdCountDto
{
    public long Id { get; set; }
    public int Count { get; set; }

    public IdCountDto() { }

    public IdCountDto(long id, int count)
    {
        Id = id;
        Count = count;
    }
}
