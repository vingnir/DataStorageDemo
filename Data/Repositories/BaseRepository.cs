using Microsoft.EntityFrameworkCore;
using Data.Contexts;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            Console.WriteLine($"DEBUG: Fetching all records for {typeof(T).Name}");
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetAsync(object id)
        {
            if (id == null || (id is int intId && intId <= 0))
            {
                throw new ArgumentException("Invalid ID value.", nameof(id));
            }

            Console.WriteLine($"DEBUG: Fetching {typeof(T).Name} with ID: {id}");
            var entity = await _dbSet.FindAsync(id);

            if (entity == null)
            {
                Console.WriteLine($"⚠️ WARNING: {typeof(T).Name} with ID {id} not found.");
            }

            return entity; // ✅ Returns `null` instead of throwing
        }

        public virtual async Task AddAsync(T entity)
        {
            Console.WriteLine($"DEBUG: Adding a new {typeof(T).Name} entity: {entity}");
            await _dbSet.AddAsync(entity);
            Console.WriteLine($"✅ SUCCESS: {typeof(T).Name} entity added successfully.");
        }

        public virtual async Task UpdateAsync(T entity)
        {
            Console.WriteLine($"DEBUG: Updating {typeof(T).Name} entity: {entity}");
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();  // ✅ Ensure persistence
            Console.WriteLine($"✅ SUCCESS: {typeof(T).Name} entity updated successfully.");
        }


        public virtual async Task DeleteAsync(object id)
        {
            Console.WriteLine($"DEBUG: Deleting {typeof(T).Name} entity with ID: {id}");
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                Console.WriteLine($"✅ SUCCESS: {typeof(T).Name} with ID {id} deleted successfully.");
            }
            else
            {
                Console.WriteLine($"⚠️ WARNING: Attempted to delete {typeof(T).Name} with ID {id}, but it was not found.");
            }
        }
    }
}
