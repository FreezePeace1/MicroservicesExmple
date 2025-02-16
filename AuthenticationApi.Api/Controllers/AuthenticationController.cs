using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Dtos;
using ECommerceSharedLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthenticationController : ControllerBase
{
    private readonly IUser _userInterface;

    public AuthenticationController(IUser userInterface)
    {
        _userInterface = userInterface;
    }

    [HttpPost("register")]
    public async Task<ActionResult<Response>> Register(AppUserDto appUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _userInterface.Register(appUserDto);

        return response.Flag ? Ok(response) : BadRequest(response);
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<Response>> Login(LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _userInterface.Login(loginDto);

        return response.Flag ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<GetUserDto>> GetUser(int id)
    {
        if (id < 0)
        {
            return BadRequest("invalid id");
        }

        var response = await _userInterface.GetUser(id);

        return response is not null ? Ok(response) : NotFound("No data");
    }

}