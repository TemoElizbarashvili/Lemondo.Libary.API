using Lemondo.Libary.API.Modules.Auth.Models;

namespace Lemondo.Libary.API.Repos.Contracts;

public interface IUserRepository : IRepository<User> 
{ 
    public Task<User?> GetByUserNameAsync(string userName);
}
