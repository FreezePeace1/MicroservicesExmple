using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Entities;

public class AppUser
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TelephoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime DateRegistered { get; set; } = DateTime.UtcNow;
}