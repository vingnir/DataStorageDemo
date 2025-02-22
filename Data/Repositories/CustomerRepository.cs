using System.Threading.Tasks;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class CustomerRepository(AppDbContext context)
    : BaseRepository<Customer>(context), ICustomerRepository
    {


    public async Task<Customer?> GetByNameAsync(string name) =>
        await _context.Customers.FirstOrDefaultAsync(c => c.Name == name);

    public async Task<Customer?> GetByIdAsync(int customerId) =>
            await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }
}
