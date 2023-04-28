using System.Linq.Expressions;
using MusicStore.Entities;
using MusicStore.Entities.Infos;

namespace MusicStore.Repositories
{
    public interface ISaleRepository : IRepositoryBase<Sale>
    {
        Task<int> CreateSaleAsync(Sale entity);

        new Task<(ICollection<TInfo> Collection, int Total)> ListAsync<TInfo, TKey>(
            Expression<Func<Sale, bool>> predicate,
            Expression<Func<Sale, TInfo>> selector,
            Expression<Func<Sale, TKey>> orderBy,
            int page, int rows);


        Task<ICollection<ReportInfo>> GetReportSaleAsync(DateTime dateStart, DateTime dateEnd);

        Task<Sale?> GetByIdAsync(int id, Expression<Func<Sale, bool>> predicate);
    }
}
