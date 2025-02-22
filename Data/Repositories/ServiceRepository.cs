using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class ServiceRepository(AppDbContext context) : BaseRepository<Service>(context), IServiceRepository
{
    public async Task<Service?> GetByNameAsync(string serviceName)
    {
        return await (_context.Services?.FirstOrDefaultAsync(s => s.Name == serviceName) ?? Task.FromResult<Service?>(null));
    }
}
