namespace CleanArchitectureTemplate.Application.DTOs.Todos;

public class CreateTodoDTO
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
}

public class TodoDTO : CreateTodoDTO
{
    public long Id { get; set; }
}
