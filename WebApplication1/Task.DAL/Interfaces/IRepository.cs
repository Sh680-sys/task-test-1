using DAL.Entities;
using System.Linq.Expressions;

namespace DAL.Interfaces;

public interface IRepository<T> where T : class
{
    System.Threading.Tasks.Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    System.Threading.Tasks.Task<List<T>> GetAllAsync(CancellationToken ct = default);
    System.Threading.Tasks.Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    System.Threading.Tasks.Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
    System.Threading.Tasks.Task GetByIdAsync(object userId, CancellationToken ct);
}