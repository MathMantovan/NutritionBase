using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutritionBase.Api.Requests.Nutritionists;
using NutritionBase.Application.Commands.Nutritionists;
using NutritionBase.Application.Queries.Nutritionists;

namespace NutritionBase.Api.Controllers;

[ApiController]
[Route("api/nutritionists")]
public class NutritionistsController : ControllerBase
{
    private readonly ISender _sender;

    public NutritionistsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateNutritionistRequest request)
    {
        var command = new CreateNutritionistCommand(request.Name, request.Email, request.Password);
        var result = await _sender.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginNutritionistRequest request)
    {
        var command = new LoginNutritionistCommand(request.Email, request.Password);
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetNutritionistByIdQuery(id);
        var result = await _sender.Send(query);
        return Ok(result);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] UpdateNutritionistRequest request)
    {
        var nutritionistId = GetNutritionistId();
        var command = new UpdateNutritionistCommand(nutritionistId, request.Name, request.Email);
        var result = await _sender.Send(command);
        return Ok(result);
    }

    private Guid GetNutritionistId() =>
        Guid.Parse(User.FindFirst("NutritionistId")!.Value);
}
