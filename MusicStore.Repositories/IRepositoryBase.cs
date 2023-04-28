using System.Linq.Expressions;
using MusicStore.Entities;

namespace MusicStore.Repositories;

public interface IRepositoryBase<TEntity> where TEntity : EntityBase
{
    
    // Listar todos los registros
    Task<ICollection<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate);

    Task<(ICollection<TInfo> Collection, int Total)> ListAsync<TInfo, TKey>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TInfo>> selector,
        Expression<Func<TEntity, TKey>> orderBy,
        int page, int rows);


    Task<ICollection<TInfo>> ListAsync<TInfo>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TInfo>> selector);

    Task<int> AddAsync(TEntity entity);

    Task<TEntity?> FindByIdAsync(int id);

    Task UpdateAsync();

    Task UpdateAsync(TEntity entity);

    Task DeleteAsync(int id);
}