using Lemondo.Libary.API.DataBase;
using Lemondo.Libary.API.Repos.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lemondo.Libary.API.Repos;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : class
{
    protected readonly AppDbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task AddAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        await _dbSet.AddAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var objectToDelete = await _dbSet.FindAsync(id);
        ArgumentNullException.ThrowIfNull(objectToDelete);
        _dbSet.Remove(objectToDelete);
    }

    public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties)
    {
        var query = ApplyIncludes(_dbSet, includeProperties);

        var entityToReturn = await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        ArgumentNullException.ThrowIfNull(entityToReturn);

        return entityToReturn!;
    }

    public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
    {
        var query = ApplyIncludes(_dbSet, includeProperties);
        return await query.ToListAsync();
    }

    public Task UpdateAsync(T entity)
        => Task.FromResult(_context.Entry(entity).State = EntityState.Modified);

    private IQueryable<T> ApplyIncludes(IQueryable<T> query, params Expression<Func<T, object>>[] includeProperties)
    {
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }
        return query;
    }

    public async Task<List<T>> ListByIdsAsync(int[] ids)
          => await _dbSet.Where(a => ids.Contains(EF.Property<int>(a, "Id")))
            .ToListAsync();
}
