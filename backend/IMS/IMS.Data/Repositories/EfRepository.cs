using IMS.Data.Data;
using IMS.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IMS.Data.Repositories;

public class EfRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;
    private readonly DbSet<T> _set;

    public EfRepository(ApplicationDbContext db)
    {
        _db = db;
        _set = _db.Set<T>();
    }

    public IQueryable<T> Query() => _set.AsQueryable();

    public async Task<T?> GetByIdAsync(params object[] keyValues)
    {
        return await _set.FindAsync(keyValues).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> ListAsync()
    {
        return await _set.ToListAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _set.Where(predicate).ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        var entry = await _set.AddAsync(entity).ConfigureAwait(false);
        return entry.Entity;
    }

    public Task UpdateAsync(T entity)
    {
        _set.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _set.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync()
    {
        return _db.SaveChangesAsync();
    }
}
