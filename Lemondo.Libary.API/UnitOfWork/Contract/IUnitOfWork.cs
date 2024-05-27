using Lemondo.Libary.API.Modules.Authors.Models;
using Lemondo.Libary.API.Modules.Books.Models;
using Lemondo.Libary.API.Repos.Contracts;

namespace Lemondo.Libary.API.UnitOfWork.Contract;

public interface IUnitOfWork
{
    public IRepository<Book> BookRepository { get; }
    public IRepository<Author> AuthorRepository { get; }
    public IUserRepository UserRepository { get; }
    Task BeginTransactionAsync();
    Task<(int, string? exceptionMessage)> CommitAsync();
    void Rollback();
    Task<int> SaveChangesAsync();
}
