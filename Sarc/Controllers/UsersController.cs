using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Sarc.Model.Entity;
using Sarc.Service.Interface;
using Sarc.DTOs;

namespace UserManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os usuários (somente admin)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Busca um usuário por ID (próprio usuário ou admin)
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUserById(string id)
    {
        // Verifica se o usuário está tentando acessar seus próprios dados ou se é admin
        if (!IsOwnerOrAdmin(id))
        {
            return Forbid();
        }

        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "Usuário não encontrado" });
        }

        return Ok(user);
    }

    /// <summary>
    /// Atualiza um usuário (próprio usuário ou admin)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> UpdateUser(string id, [FromBody] UpdateUserDto dto)
    {
        if (!IsOwnerOrAdmin(id))
        {
            return Forbid();
        }

        var user = await _userService.UpdateUserAsync(id, dto);
        if (user == null)
        {
            return NotFound(new { message = "Usuário não encontrado" });
        }

        return Ok(user);
    }

    /// <summary>
    /// Atualiza parcialmente um usuário (próprio usuário ou admin)
    /// </summary>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> PatchUser(string id, [FromBody] UpdateUserDto dto)
    {
        return await UpdateUser(id, dto);
    }

    /// <summary>
    /// Remove um usuário (somente admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var deleted = await _userService.DeleteUserAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = "Usuário não encontrado" });
        }

        return NoContent();
    }

    private bool IsOwnerOrAdmin(string userId)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value;
        var isAdmin = User.HasClaim("cognito:groups", "admin");

        return isAdmin || currentUserId == userId;
    }
}