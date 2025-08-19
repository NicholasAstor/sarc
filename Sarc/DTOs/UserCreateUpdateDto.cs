using System.ComponentModel.DataAnnotations;

namespace Sarc.DTOs;

public class UserCreateUpdateDto
{
    [Required, StringLength(80)]
    public string Name { get; set; } = default!;

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = default!;
}
