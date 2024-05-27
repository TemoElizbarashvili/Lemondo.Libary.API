using AutoMapper;
using Lemondo.Libary.API.Modules.Auth.Models;
using Lemondo.Libary.API.Modules.Auth.Service.Contract;
using Lemondo.Libary.API.Modules.Shared.Controller;
using Lemondo.Libary.API.UnitOfWork.Contract;
using Microsoft.AspNetCore.Mvc;

namespace Lemondo.Libary.API.Modules.Auth;

[Route("[controller]")]
[ApiController]
public class AuthController(IUnitOfWork uow, IMapper mapper, IAuthService authService) : BaseController(uow, mapper)
{
    private readonly IAuthService _authService = authService;

    [HttpPost("register")]
    [ProducesResponseType(201)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Register(UserDto request)
    {
        var user = _mapper.Map<User>(request);
        user.Password = await _authService.HashPasswordAsync(user.Password);
        try
        {
            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Conflict(new { message = ex.InnerException?.Message });
        }

        return CreatedAtAction(nameof(Register), user.Id);
    }

    [HttpPost("login")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<string>> Login(UserDto request)
    {
        var user = await _unitOfWork.UserRepository.GetByUserNameAsync(request.UserName);

        if (user is null)
            return BadRequest("User name or password is incorect");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return BadRequest("User name or password is incorect");

        var token = await _authService.GenerateTokenAsync(user);

        return Ok(token);
    }
}
