using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStore.DataAccess;
using MusicStore.Entities;

namespace MusicStore.Repositories;

public class ConcertRepository : RepositoryBase<Concert>, IConcertRepository
{
    public ConcertRepository(MusicStoreDbContext context)
        :base(context)
    {
    }

    /// <summary>
    /// Esta funcion trae todos los conciertos y agregando la clase de Genre
    /// </summary>
    public override async Task<(ICollection<TInfo> Collection, int Total)> ListAsync<TInfo, TKey>(Expression<Func<Concert, bool>> predicate, Expression<Func<Concert, TInfo>> selector, Expression<Func<Concert, TKey>> orderBy, int page, int rows)
    {
        var collection = await Context.Set<Concert>()
            .Include(p => p.Genre) // Eager Loading
            .Where(predicate)
            .OrderByDescending(orderBy)
            .Skip((page - 1) * rows)
            .Take(rows)
            .AsNoTracking()
            .Select(selector)
            .ToListAsync();

        var total = await Context.Set<Concert>()
            .Where(predicate)
            .CountAsync();

        return (collection, total);
    }

    public override async Task<Concert?> FindByIdAsync(int id)
    {
        return await Context.Set<Concert>()
            .Include(p => p.Genre) // Eager Loading
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task FinalizeAsync(int id)
    {
        // Modelo Conectado. EF Core tiene el registro en memoria

        var entity = await Context.Set<Concert>().SingleOrDefaultAsync(x => x.Id == id
            && x.Status);

        if (entity == null) throw new InvalidOperationException("No se encontró el concierto");

        entity.Finalized = true;
        await UpdateAsync();

    }
}