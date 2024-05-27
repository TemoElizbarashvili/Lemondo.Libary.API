using Lemondo.Libary.API.Modules.Auth.Models;

namespace Lemondo.Libary.API.Modules.Auth.Service.Contract;

public interface IAuthService
{
    public Task<string> HashPasswordAsync(string password);
    public Task<string> GenerateTokenAsync(User user);
}
