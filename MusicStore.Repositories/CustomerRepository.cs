using Microsoft.EntityFrameworkCore;
using MusicStore.DataAccess;
using MusicStore.Entities;

namespace MusicStore.Repositories;

public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
{
    public CustomerRepository(MusicStoreDbContext context) : base(context)
    {
        
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await Context.Set<Customer>()
            .FirstOrDefaultAsync(c => c.Email == email);
    }
}