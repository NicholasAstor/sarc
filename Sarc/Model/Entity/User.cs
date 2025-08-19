using System.ComponentModel.DataAnnotations;

namespace Sarc.Model.Entity;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, StringLength(80)]
    public string Name { get; set; } = default!;

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
