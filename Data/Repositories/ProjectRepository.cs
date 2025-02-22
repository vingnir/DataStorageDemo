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
            var projects = await _context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Status)
                .Include(p => p.Service)
                .Include(p => p.Staff)
                .ThenInclude(s => s.Role) // ✅ Ensure role is included
                .ToListAsync();

            foreach (var project in projects)
            {
                Console.WriteLine($"🔍 DEBUG: Project '{project.ProjectNumber}', Staff '{project.Staff?.Name}', Role '{project.Staff?.Role?.Name}'");
            }

            return projects ?? Enumerable.Empty<Project>();
        }


        public override async Task<Project?> GetAsync(object id)
        {
            var projectNumber = (string)id;

            var project = await _context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Status)
                .Include(p => p.Service)
                .Include(p => p.Staff)
                .ThenInclude(s => s.Role)  // ✅ No need for explicit null check
                .FirstOrDefaultAsync(p => p.ProjectNumber == projectNumber);

            if (project == null)
            {
                Console.WriteLine($"⚠️ WARNING: Project with Number '{projectNumber}' not found.");
            }

            return project;
        }
    }
}
