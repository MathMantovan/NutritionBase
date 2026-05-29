using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutritionBase.Api.Requests.MealPlans;
using NutritionBase.Application.Commands.MealPlans;
using NutritionBase.Application.Queries.MealPlans;

namespace NutritionBase.Api.Controllers;

[ApiController]
[Route("api/mealplans")]
[Authorize]
public class MealPlansController : ControllerBase
{
    private readonly ISender _sender;

    public MealPlansController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMealPlanRequest request)
    {
        var command = new CreateMealPlanCommand(
            request.PatientId,
            request.Name,
            request.Objective,
            request.StartDate,
            request.EndDate);

        var result = await _sender.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetMealPlanByIdQuery(id);
        var result = await _sender.Send(query);
        return Ok(result);
    }

    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> ListByPatient(Guid patientId)
    {
        var query = new ListMealPlansByPatientQuery(patientId);
        var result = await _sender.Send(query);
        return Ok(result);
    }

    [HttpPost("{id:guid}/meals")]
    public async Task<IActionResult> AddMeal(Guid id, [FromBody] AddMealRequest request)
    {
        var command = new AddMealCommand(id, request.Name, request.MealTime, request.FoodItems);
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpDelete("{mealPlanId:guid}/meals/{mealId:guid}")]
    public async Task<IActionResult> RemoveMeal(Guid mealPlanId, Guid mealId)
    {
        var command = new RemoveMealCommand(mealPlanId, mealId);
        await _sender.Send(command);
        return NoContent();
    }
}
