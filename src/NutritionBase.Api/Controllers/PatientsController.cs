using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutritionBase.Api.Requests.Patients;
using NutritionBase.Application.Commands.Patients;
using NutritionBase.Application.Queries.Patients;

namespace NutritionBase.Api.Controllers;

[ApiController]
[Route("api/patients")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly ISender _sender;

    public PatientsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientRequest request)
    {
        var command = new CreatePatientCommand(
            GetNutritionistId(),
            request.Name,
            request.Email,
            request.AreaCode,
            request.PhoneNumber,
            request.BirthDate,
            request.Weight,
            request.Height);

        var result = await _sender.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var query = new ListPatientsQuery(GetNutritionistId());
        var result = await _sender.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetPatientByIdQuery(id);
        var result = await _sender.Send(query);
        return Ok(result);
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var query = new GetPatientsByNameQuery(name, GetNutritionistId());
        var result = await _sender.Send(query);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientRequest request)
    {
        var command = new UpdatePatientCommand(
            id,
            GetNutritionistId(),
            request.Name,
            request.Email,
            request.AreaCode,
            request.PhoneNumber,
            request.BirthDate,
            request.Weight,
            request.Height);

        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        var command = new RemovePatientCommand(id);
        await _sender.Send(command);
        return NoContent();
    }

    private Guid GetNutritionistId() =>
        Guid.Parse(User.FindFirst("NutritionistId")!.Value);
}
