using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sarc.DTOs;
using Sarc.Model.Entity;
using Sarc.Service.Interface;

namespace Sarc.Controllers;

[ApiController]
[Route("api/v1/rooms")]
[Authorize(Policy = "AdminOnly")] // só admin pode mexer em salas
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    /// <summary>
    /// Lista salas com paginação e filtro opcional por capacidade mínima.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetRooms(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? minCapacity = null)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest(new { error = "page e pageSize devem ser maiores que zero." });
        }

        var rooms = await _roomService.GetRoomsAsync(page, pageSize, minCapacity);
        return Ok(rooms);
    }

    /// <summary>
    /// Busca uma sala pelo Id.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetRoomById(string id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null)
        {
            return NotFound();
        }

        return Ok(room);
    }

    /// <summary>
    /// Cria uma nova sala.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Room>> CreateRoom([FromBody] CreateRoomDto dto)
    {
        if (dto.Capacity <= 0 || string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new { error = "Nome é obrigatório e capacidade deve ser maior que zero." });
        }

        var room = await _roomService.CreateAsync(dto);
        return Created($"/api/v1/rooms/{room.Id}", room);
    }

    /// <summary>
    /// Atualiza dados de uma sala.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Room>> UpdateRoom(string id, [FromBody] UpdateRoomDto dto)
    {
        var updated = await _roomService.UpdateAsync(id, dto);
        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    /// <summary>
    /// Remove uma sala.
    ///</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(string id)
    {
        var deleted = await _roomService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
