using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sarc.DTOs;
using Sarc.Model.Entity;
using Sarc.Service.Interface;

namespace Sarc.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize] // precisa estar autenticado para mexer em reservas
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public SchedulesController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? User.FindFirst("sub")?.Value;
    }

    private bool IsAdmin()
    {
        return User.HasClaim("cognito:groups", "admin");
    }

    /// <summary>
    /// Cria uma nova reserva de sala para o usuário autenticado.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Schedule>> CreateSchedule([FromBody] CreateScheduleDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "Usuário não identificado no token." });
        }

        try
        {
            var schedule = await _scheduleService.CreateAsync(userId, dto);
            return Created($"/api/v1/schedules/{schedule.Id}", schedule);
        }
        catch (ArgumentException ex)
        {
            // problemas de validação de entrada (datas inválidas, sala inválida...)
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // conflitos de horário
            return Conflict(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Lista as reservas do usuário autenticado.
    /// </summary>
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<Schedule>>> GetMySchedules()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "Usuário não identificado no token." });
        }

        var schedules = await _scheduleService.GetMySchedulesAsync(userId);
        return Ok(schedules);
    }

    /// <summary>
    /// Cancela uma reserva. Apenas o dono ou admin podem cancelar.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelSchedule(string id)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "Usuário não identificado no token." });
        }

        var isAdmin = IsAdmin();

        try
        {
            var result = await _scheduleService.CancelAsync(id, userId, isAdmin);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
