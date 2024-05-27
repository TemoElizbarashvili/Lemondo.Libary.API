using Lemondo.Libary.API.DataBase;
using Lemondo.Libary.API.Modules.Auth.Models;
using Lemondo.Libary.API.Repos.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Lemondo.Libary.API.Repos;

public class UserRepository(AppDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByUserNameAsync(string userName)
        => await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
}
