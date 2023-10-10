using CleanArchitectureTemplate.Domain.SeedWork;

namespace CleanArchitectureTemplate.Domain.Aggregates.ToDos;

public class Todo : AggregateRoot
{
    public string Title { get; protected set; }
    public string Description { get; protected set; }
    public bool IsCompleted { get; protected set; }
    public DateTime? CompletedDate { get; protected set; }

    protected Todo() { }

    public Todo(
        long id,
        string title,
        string description,
        bool isCompleted,
        DateTime? completedDate) : base(id)
    {
        Title = title;
        Description = description;
        IsCompleted = isCompleted;
        CompletedDate = completedDate;
    }

    public Todo(
        string title,
        string description,
        bool isCompleted,
        DateTime? completedDate)
    {
        Title = title;
        Description = description;
        IsCompleted = isCompleted;
        CompletedDate = completedDate;
    }

    public void SetTitle(string title)
    {
        Title = title;
    }

    public void SetDescription(string description)
    {
        Description = description;
    }

    public void SetIsCompleted(bool isCompleted)
    {
        IsCompleted = isCompleted;
    }

    public void SetCompletedDate(DateTime? completedDate)
    {
        CompletedDate = completedDate;
    }
}
