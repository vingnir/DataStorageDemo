using Business.Dtos;
using Business.Interfaces;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class RoleService(
        IRoleRepository roleRepo,
        IUnitOfWork unitOfWork,
        IDbContextFactory<AppDbContext> dbContextFactory
    ) : IRoleService
    {
        private readonly IRoleRepository _roleRepo = roleRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory = dbContextFactory;

        // ✅ Ensure Role Exists (Using Transactions)
        public async Task<int> EnsureRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be empty.", nameof(roleName));

            Console.WriteLine($"🔍 [EnsureRoleAsync] Checking if role '{roleName}' exists...");

            var existingRole = await _roleRepo.GetByNameAsync(roleName);
            if (existingRole != null)
            {
                Console.WriteLine($"✅ [EnsureRoleAsync] Role '{roleName}' exists (ID: {existingRole.Id}).");
                return existingRole.Id;  // No transaction needed if found
            }

            Console.WriteLine($"❌ [EnsureRoleAsync] Role '{roleName}' not found, creating new one...");

            bool startedTransaction = false;
            if (!_unitOfWork.HasActiveTransaction)
            {
                await _unitOfWork.BeginTransactionAsync();
                Console.WriteLine("🔵 [EnsureRoleAsync] Started new transaction.");
                startedTransaction = true;
            }

            try
            {
                var newRole = new Role { Name = roleName };
                await _roleRepo.AddAsync(newRole);

                if (startedTransaction)
                {
                    await _unitOfWork.CommitAsync();
                    Console.WriteLine($"✅ [EnsureRoleAsync] Created Role '{roleName}' with ID {newRole.Id}.");
                }

                return newRole.Id;
            }
            catch
            {
                if (startedTransaction) await _unitOfWork.RollbackAsync();
                Console.WriteLine($"❌ [EnsureRoleAsync] Failed to create role '{roleName}'.");
                throw;
            }
        }

        // ✅ Get Role by Name
        public async Task<int> GetRoleIdByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be empty.", nameof(roleName));

            Console.WriteLine($"🔍 [GetRoleIdByNameAsync] Searching for role '{roleName}'...");

            var role = await _roleRepo.GetByNameAsync(roleName);
            if (role == null)
            {
                Console.WriteLine($"❌ [GetRoleIdByNameAsync] Role '{roleName}' does not exist.");
                throw new ArgumentException($"Role '{roleName}' does not exist.");
            }

            Console.WriteLine($"✅ [GetRoleIdByNameAsync] Found role '{roleName}' with ID {role.Id}.");
            return role.Id;
        }

        // ✅ Get All Roles (Using Factory for Read-Only Query)
        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            Console.WriteLine("🔍 [GetAllRolesAsync] Fetching all roles...");

            using var dbContext = _dbContextFactory.CreateDbContext();
            var roles = await dbContext.Roles.ToListAsync();

            Console.WriteLine($"✅ [GetAllRolesAsync] Found {roles.Count} roles.");

            return roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
        }
    }
}
