using MusicStore.Entities;

namespace MusicStore.Repositories;

public interface IConcertRepository : IRepositoryBase<Concert>
{
    Task FinalizeAsync(int id);
}