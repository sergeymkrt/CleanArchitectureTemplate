using CleanArchitectureTemplate.Application.DTOs.Users;
using CleanArchitectureTemplate.Application.UseCases.Auth.Commands;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : BaseApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
    {
        return Ok(await Mediator.Send(new RegisterUserCommand(userRegisterDto)));
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserAuthDto userAuthDto)
    {
        var token = await Mediator.Send(new LoginUserCommand(userAuthDto));
        
        Response.Cookies.Append("authorization", token.Data, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/"
        });
        
        return Ok();
    }
}
