using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Dtos;

public record AppUserDto
(
    int Id,
    [Required] string Name,
    [Required] string TelephoneNumber,
    [Required] string Address,
    [Required,EmailAddress] string Email,
    [Required] string Password,
    [Required] string Role
);
