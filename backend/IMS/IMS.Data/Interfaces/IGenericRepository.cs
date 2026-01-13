using System.Linq.Expressions;

namespace IMS.Data.Interfaces;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> Query();

    Task<T?> GetByIdAsync(params object[] keyValues);

    Task<IEnumerable<T>> ListAsync();

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    Task<T> AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);

    Task<int> SaveChangesAsync();
}
