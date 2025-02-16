using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Dtos;

public record LoginDto([Required,EmailAddress] string Email,[Required] string Password);
