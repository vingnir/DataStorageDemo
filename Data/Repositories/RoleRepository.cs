using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class RoleRepository(AppDbContext context) : BaseRepository<Role>(context), IRoleRepository
{
    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await (_context.Roles?.FirstOrDefaultAsync(r => r.Name == roleName) ?? Task.FromResult<Role?>(null));
    }

}
