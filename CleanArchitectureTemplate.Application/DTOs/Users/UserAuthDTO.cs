namespace CleanArchitectureTemplate.Application.DTOs.Users;

public class UserAuthDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UserRegisterDto : UserAuthDto
{
    public string ConfirmPassword { get; set; }
}
