using Microsoft.EntityFrameworkCore;
using MusicStore.DataAccess;
using MusicStore.Entities;
namespace MusicStore.Repositories;

public class GenreRepository : RepositoryBase<Genre>, IGenreRepository
{
    public GenreRepository(MusicStoreDbContext context)
        :base(context)
    {
    }

    public async Task<List<Genre>> ListAsync()
    {
        return await Context.Set<Genre>()
            .Where(p => p.Status)
            .AsNoTracking()
            .ToListAsync();
    }

}