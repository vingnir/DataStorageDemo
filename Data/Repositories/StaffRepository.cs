using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class StaffRepository(AppDbContext context) : BaseRepository<Staff>(context), IStaffRepository
    {
        public async Task<Staff?> GetByNameAndRoleIdAsync(string staffName, int roleId)
        {
            Console.WriteLine($"DEBUG: Searching for existing Staff with Name = '{staffName}' and RoleId = {roleId}");

            var staff = await _dbSet.FirstOrDefaultAsync(s => s.Name == staffName && s.RoleId == roleId);

            if (staff != null)
            {
                Console.WriteLine($"✅ DEBUG: Found existing Staff '{staff.Name}' with RoleId {roleId}, ID: {staff.StaffId}");
            }
            else
            {
                Console.WriteLine($"⚠️ WARNING: No Staff found with Name = '{staffName}' and RoleId = {roleId}");
            }

            return staff;
        }
    }
}
