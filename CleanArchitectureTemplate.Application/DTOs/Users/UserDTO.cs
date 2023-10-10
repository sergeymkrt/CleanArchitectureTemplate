namespace CleanArchitectureTemplate.Application.DTOs.Users;

public class UserDto
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{LastName} {FirstName}";
    public string Email { get; set; }
}
