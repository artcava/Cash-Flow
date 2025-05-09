using CashFlow.Data;
using CashFlow.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CashFlow.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    protected readonly CashFlowContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(CashFlowContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity> GetByIdAsync(int id, Action<IQueryable<TEntity>> options = null, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;

        query = includes.Aggregate(query, (current, include) => current.Include(include));

        options?.Invoke(query);

        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllWithIncludeAsync(params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        return await query.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}