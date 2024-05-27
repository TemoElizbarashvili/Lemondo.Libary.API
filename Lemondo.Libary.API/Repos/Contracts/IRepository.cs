using System.Linq.Expressions;

namespace Lemondo.Libary.API.Repos.Contracts;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties);
    public Task AddAsync(T entity);
    public Task UpdateAsync(T entity);
    public Task DeleteAsync(int id);
    public Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
    public Task<List<T>> ListByIdsAsync(int[] ids);
}
