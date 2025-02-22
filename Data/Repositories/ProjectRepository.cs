using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ProjectRepository(AppDbContext context) : BaseRepository<Project>(context), IProjectRepository
    {
        public override async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Status)
                .Include(p => p.Service)
                .Include(p => p.Staff)
                .ThenInclude(s => s != null ? s.Role : null)
                .ToListAsync() ?? Enumerable.Empty<Project>();
        }

        public override async Task<Project?> GetAsync(object id)
        {
            var projectNumber = (string)id;

            var project = await _context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Status)
                .Include(p => p.Service)
                .Include(p => p.Staff)
                    .ThenInclude(s => s != null ? s.Role : null)  // ✅ Fix: Prevent null access
                .FirstOrDefaultAsync(p => p.ProjectNumber == projectNumber);

            if (project == null)
            {
                Console.WriteLine($"⚠️ WARNING: Project with Number '{projectNumber}' not found.");
            }

            return project;
        }

    }
}
