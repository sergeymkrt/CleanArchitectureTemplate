using CleanArchitectureTemplate.Domain.Enums;

namespace CleanArchitectureTemplate.Application.DTOs;

public class PaginationDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public List<(string, bool)>? OrderBy { get; set; }
}
