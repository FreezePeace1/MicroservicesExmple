using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.DAL.Data;
using AuthenticationApi.Domain.Dtos;
using AuthenticationApi.Domain.Entities;
using ECommerceSharedLibrary.Logs;
using ECommerceSharedLibrary.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationApi.DAL.Repository;

public class UserRepository : IUser
{
    private readonly AuthenticationDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public UserRepository(AuthenticationDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<Response> Register(AppUserDto appUserDto)
    {
        try
        {
            var getUser = await GetUserByEmail(appUserDto.Email);

            if (getUser is not null)
            {
                return new Response($"you can not use this email {appUserDto.Email}" +
                                    $"because it is already registered!");
            }

            var newUser = await _dbContext.Users.AddAsync(new AppUser()
            {
                Name = appUserDto.Name,
                Address = appUserDto.Address,
                DateRegistered = DateTime.UtcNow,
                Email = appUserDto.Email,
                Role = appUserDto.Role,
                TelephoneNumber = appUserDto.TelephoneNumber,
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDto.Password)
            });

            await _dbContext.SaveChangesAsync();

            return new Response($"User named {newUser.Entity.Name} is registered!",true);
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);

            return new Response("Can not register exception");
        }
    }

    public async Task<Response> Login(LoginDto loginDto)
    {
        try
        {
            var existedUser = await GetUserByEmail(loginDto.Email);

            if (existedUser is null)
            {
                return new Response($"User with {loginDto.Email} email does not exist");
            }

            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, existedUser.Password);

            if (!verifyPassword)
            {
                return new Response($"Invalid credentials");
            }

            string token = GenerateToken(existedUser);

            return new Response($"{token}", true);
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);

            return new Response("Can not login exception");
        }
    }

    private string GenerateToken(AppUser user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration.GetSection("Authentication:Key")
            .Value!);

        var secretKey = new SymmetricSecurityKey(key);

        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        if (!string.IsNullOrEmpty(user.Role) || !Equals("string", user.Role))
        {
         claims.Add(new (ClaimTypes.Role,user.Role));   
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Authentication:Issuer"],
            audience: _configuration["Authentication:Audience"],
            claims: claims,
            expires: null,
            signingCredentials: credentials
            );

        string tkn = new JwtSecurityTokenHandler().WriteToken(token);
        
        return tkn;
    }

    
    private async Task<AppUser> GetUserByEmail(string email)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

            return user is not null ? user : null!;
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);

            return null!;
        }
    }

    public async Task<GetUserDto> GetUser(int userId)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

            return user is not null
                ? new GetUserDto
                (
                    user.Id,
                    user.Name,
                    user.TelephoneNumber,
                    user.Address,
                    user.Email,
                    user.Role
                )
                : null!;
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);

            return null!;
        }
    }
}