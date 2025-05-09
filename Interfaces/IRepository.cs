using System.Linq.Expressions;

namespace CashFlow.Interfaces;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    Task<TEntity> GetByIdAsync(int id, Action<IQueryable<TEntity>> options = null, params Expression<Func<TEntity, object>>[] includes);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> GetAllWithIncludeAsync(params Expression<Func<TEntity, object>>[] includes);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(int id);
}
