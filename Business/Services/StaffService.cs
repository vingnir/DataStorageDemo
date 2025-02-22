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

namespace Business.Services.Implementations
{
    public class StaffService(
        IStaffRepository staffRepo,
        IRoleService roleService,
        IUnitOfWork unitOfWork,  // ✅ Use UnitOfWork instead of DbContext
        IDbContextFactory<AppDbContext> dbContextFactory // ✅ Use Factory for Read-Only Queries
    ) : IStaffService
    {
        private readonly IStaffRepository _staffRepo = staffRepo;
        private readonly IRoleService _roleService = roleService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory = dbContextFactory;

        // ✅ Ensure Staff Exists (Using Transactions)
        public async Task<int> EnsureStaffAsync(StaffDto staffDto)
        {
            if (staffDto == null)
                throw new ArgumentException("❌ Staff details cannot be null.");

            Console.WriteLine($"DEBUG: Checking Role '{staffDto.RoleName}' for Staff '{staffDto.Name}'");

            // 🛠 **Ensure Role Exists First**
            int roleId = await _roleService.EnsureRoleAsync(staffDto.RoleName);
            if (roleId <= 0)
            {
                Console.WriteLine($"❌ ERROR: Role '{staffDto.RoleName}' does not exist and could not be created.");
                throw new InvalidOperationException($"Role '{staffDto.RoleName}' does not exist.");
            }
            Console.WriteLine($"✅ DEBUG: Role '{staffDto.RoleName}' has ID {roleId}");

            // 🛠 **Check if Staff Already Exists**
            Console.WriteLine($"DEBUG: Checking if Staff '{staffDto.Name}' with RoleId {roleId} exists...");
            var existingStaff = await _staffRepo.GetByNameAndRoleIdAsync(staffDto.Name, roleId);

            if (existingStaff != null)
            {
                Console.WriteLine($"✅ DEBUG: Staff '{staffDto.Name}' already exists with RoleId {roleId}, returning existing StaffId: {existingStaff.StaffId}");
                return existingStaff.StaffId;
            }

            // 🚀 **Create New Staff ONLY IF IT DOESN'T EXIST**
            Console.WriteLine($"❌ DEBUG: Staff '{staffDto.Name}' does not exist, creating new entry...");

            var newStaff = new Staff
            {
                Name = staffDto.Name,
                RoleId = roleId,  // ✅ Ensure RoleId is set correctly
            };

            bool startedTransaction = false;
            if (!_unitOfWork.HasActiveTransaction)
            {
                await _unitOfWork.BeginTransactionAsync();
                Console.WriteLine("🔵 [EnsureStaffAsync] Started new transaction.");
                startedTransaction = true;
            }

            try
            {
                await _staffRepo.AddAsync(newStaff);
                if (startedTransaction)
                {
                    await _unitOfWork.CommitAsync();
                    Console.WriteLine($"✅ [EnsureStaffAsync] Created new Staff '{staffDto.Name}' with ID {newStaff.StaffId} and RoleId {newStaff.RoleId}");
                }

                return newStaff.StaffId;
            }
            catch (Exception ex)
            {
                if (startedTransaction)
                {
                    await _unitOfWork.RollbackAsync();
                    Console.WriteLine($"❌ [EnsureStaffAsync] Transaction rolled back due to an error: {ex.Message}");
                }
                throw;
            }
        }



        // ✅ Check if Staff Exists
        public async Task<bool> CheckStaffExistsAsync(int staffId)
        {
            var staff = await _staffRepo.GetAsync(staffId);
            return staff != null;
        }

        // ✅ Get All Staff (Mapping to DTO)
        public async Task<IEnumerable<StaffDto>> GetAllStaffAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();  // ✅ Use Factory for short-lived queries
            var staffList = await dbContext.Staff.Include(s => s.Role).ToListAsync();

            return staffList.Select(s => new StaffDto
            {
                StaffId = s.StaffId,
                Name = s.Name,
                RoleName = s.Role?.Name ?? "No Role"
            }).ToList();  // ✅ Convert to DTO
        }
    }
}
