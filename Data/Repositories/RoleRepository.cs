using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class RoleRepository(AppDbContext context) : BaseRepository<Role>(context), IRoleRepository
{
    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await _context.Roles
            .AsNoTracking() // ✅ Prevents caching issues
            .FirstOrDefaultAsync(r => r.Name == roleName);
    }

    // ✅ New Method: Get Role by ID
    public async Task<Role?> GetRoleByIdAsync(int roleId)
    {
        return await _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == roleId);
    }
}
