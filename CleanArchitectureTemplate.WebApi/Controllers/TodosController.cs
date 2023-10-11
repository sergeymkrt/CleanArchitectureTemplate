using CleanArchitectureTemplate.Application.DTOs;
using CleanArchitectureTemplate.Application.DTOs.Todos;
using CleanArchitectureTemplate.Application.UseCases.Todos.Commands;
using CleanArchitectureTemplate.Application.UseCases.Todos.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodosController : BaseApiController
{
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoAsync(long id)
    {
        return Ok(await Mediator.Send(new DeleteTodoCommand(id)));
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTodosAsync([FromQuery] PaginationDto dto)
    {
        return Ok(Mediator.Send(new GetTodosQuery(dto)));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTodoAsync([FromBody] CreateTodoDTO dto)
    {
        return Ok(Mediator.Send(new CreateTodoCommand(dto)));
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoAsync(long id, [FromBody] TodoDTO dto)
    {
        if (id != dto.Id)
            return BadRequest();
        
        return Ok(Mediator.Send(new UpdateTodoCommand(dto)));
    }
}
