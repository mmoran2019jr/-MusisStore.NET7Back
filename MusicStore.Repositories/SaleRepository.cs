using System.Data;
using System.Linq.Expressions;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MusicStore.DataAccess;
using MusicStore.Entities;
using MusicStore.Entities.Infos;

namespace MusicStore.Repositories;

public class SaleRepository : RepositoryBase<Sale>, ISaleRepository
{
    public SaleRepository(MusicStoreDbContext context) : base(context)
    {
        
    }

    public async Task<int> CreateSaleAsync(Sale entity)
    {
        entity.SaleDate = DateTime.Now;
        var lastNumber = await Context.Set<Sale>().CountAsync() + 1;
        entity.OperationNumber = $"{lastNumber:000000}"; // 000001

        await Context.Set<Sale>().AddAsync(entity);
        await Context.SaveChangesAsync();

        return entity.Id;
    }

    public override async Task<(ICollection<TInfo> Collection, int Total)> ListAsync<TInfo, TKey>(Expression<Func<Sale, bool>> predicate, Expression<Func<Sale, TInfo>> selector, Expression<Func<Sale, TKey>> orderBy, int page, int rows)
    {
        var collection = await Context.Set<Sale>()
            .Include(x => x.Customer)
            .Include(x => x.Concert)
            .ThenInclude(x => x.Genre)
            .Where(predicate)
            .OrderBy(orderBy)
            .Skip((page - 1) * rows)
            .Take(rows)
            .AsNoTracking()
            .Select(selector)
            .ToListAsync();

        var total = await Context.Set<Sale>()
            .Where(predicate)
            .CountAsync();

        return (collection, total);
    }

    public async Task<ICollection<ReportInfo>> GetReportSaleAsync(DateTime dateStart, DateTime dateEnd)
    {
        // Esto es con EF Core
        //var query = Context.Set<ReportInfo>()
        //    .FromSqlRaw("EXEC uspReportSales {0},{1}", dateStart, dateEnd);

        //return await query.ToListAsync();

        // Esto es con ADO.NET
        //using (var connection = new SqlConnection(Context.Database.GetConnectionString()))
        //{
        //    using (var command = connection.CreateCommand())
        //    {
        //        command.CommandText = "uspReportSales";
        //        command.CommandType = CommandType.StoredProcedure;

        //        command.Parameters.AddWithValue("@DateStart", dateStart);
        //        command.Parameters.AddWithValue("@DateEnd", dateEnd);

        //        await connection.OpenAsync();

        //        using (var dr = await command.ExecuteReaderAsync())
        //        {
        //            var collection = new List<ReportInfo>();

        //            while (await dr.ReadAsync())
        //            {
        //                var reportInfo = new ReportInfo
        //                {
        //                    ConcertName= dr.GetString(0),
        //                    Total = dr.GetDecimal(1)
        //                };

        //                collection.Add(reportInfo);
        //            }

        //            return collection;
        //        }
        //    }
        //}

        // Esto es con Dapper
        using (var connection = new SqlConnection(Context.Database.GetConnectionString()))
        {
            var parameters = new DynamicParameters();
            parameters.Add("@DateStart", dateStart);
            parameters.Add("@DateEnd", dateEnd);

            return (await connection.QueryAsync<ReportInfo>("uspReportSales", parameters, commandType: CommandType.StoredProcedure)).ToList();
        }

    }

    public async Task<Sale?> GetByIdAsync(int id, Expression<Func<Sale, bool>> predicate)
    {
        return await Context.Set<Sale>()
            .Include(p => p.Concert)
            .ThenInclude(p => p.Genre)
            .Include(p => p.Customer)
            .Where(predicate)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
}