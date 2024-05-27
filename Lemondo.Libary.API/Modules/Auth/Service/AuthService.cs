using Lemondo.Libary.API.Modules.Auth.Models;
using Lemondo.Libary.API.Modules.Auth.Service.Contract;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lemondo.Libary.API.Modules.Auth.Service;

public class AuthService(IConfiguration config) : IAuthService
{
    private readonly IConfiguration _config = config;

    public Task<string> GenerateTokenAsync(User user)
    {
        List<Claim> claims = new() { new Claim(ClaimTypes.Name, user.UserName) };
        var keyStying = _config.GetSection("JwtSettings:Key").Value!;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JwtSettings:Key").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: creds
            );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Task.FromResult(jwt);
    }

    public Task<string> HashPasswordAsync(string password)
        => Task.FromResult(BCrypt.Net.BCrypt.HashPassword(password));
}
