using Lemondo.Libary.API.DataBase;
using Lemondo.Libary.API.Modules.Authors.Models;
using Lemondo.Libary.API.Modules.Books.Models;
using Lemondo.Libary.API.Repos;
using Lemondo.Libary.API.Repos.Contracts;
using Lemondo.Libary.API.UnitOfWork.Contract;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lemondo.Libary.API.UnitOfWork;

public class UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<UnitOfWork> _logger = logger;
    private IDbContextTransaction? _transaction;
    public IRepository<Book> BookRepository => new Repository<Book>(_context);
    public IRepository<Author> AuthorRepository => new Repository<Author>(_context);
    public IUserRepository UserRepository => new UserRepository(_context);


    public async Task BeginTransactionAsync()
        => _transaction = await _context.Database.BeginTransactionAsync();

    public async Task<(int, string? exceptionMessage)> CommitAsync()
    {
        if (_transaction is null)
            return (-1, "Transaction is not started correctly!");
        try
        {
            var result = await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
            return (result, null);
        }
        catch (Exception ex)
        {
            await _transaction.RollbackAsync();
            _logger.LogError(ex.ToString());
            return (-1, ex.ToString());
        }
        finally
        {
            await _transaction.DisposeAsync();
        }
    }

    public void Rollback()
    {
        if (_transaction is null) return;
        _transaction.Rollback();
    }

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
