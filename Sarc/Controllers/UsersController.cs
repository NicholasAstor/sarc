using Microsoft.AspNetCore.Mvc;
using Sarc.DTOs;
using Sarc.Model.Entity;

namespace Sarc.Controllers;

[ApiController]
[Route("api/[controller]")] // /api/users
public class UsersController : ControllerBase
{
    // Armazenamento em memória só para demo/Sprint 0
    private static readonly List<User> _users = new();

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAll() => Ok(_users);

    [HttpGet("{id:guid}")]
    public ActionResult<User> GetById(Guid id)
    {
        var u = _users.FirstOrDefault(x => x.Id == id);
        return u is null ? NotFound() : Ok(u);
    }

    [HttpPost]
    public ActionResult<User> Create([FromBody] UserCreateUpdateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        if (_users.Any(u => u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
            return Conflict(new { message = "Email already in use." });

        var user = new User { Name = dto.Name, Email = dto.Email };
        _users.Add(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<User> Update(Guid id, [FromBody] UserCreateUpdateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var user = _users.FirstOrDefault(x => x.Id == id);
        if (user is null) return NotFound();

        if (_users.Any(u => u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase) && u.Id != id))
            return Conflict(new { message = "Email already in use." });

        user.Name = dto.Name;
        user.Email = dto.Email;
        return Ok(user);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var user = _users.FirstOrDefault(x => x.Id == id);
        if (user is null) return NotFound();

        _users.Remove(user);
        return NoContent();
    }
}
