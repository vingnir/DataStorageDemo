using Business.Dtos;
using Business.Interfaces;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ProjectService(
        IProjectRepository projectRepo,
        IStaffService staffService,
        IServiceService serviceService,
        ICustomerService customerService,
        IUnitOfWork unitOfWork,  // ✅ Use UnitOfWork for Transactions
        IDbContextFactory<AppDbContext> dbContextFactory  // ✅ Use Factory for Read-Only Queries
    ) : IProjectService
    {
        private readonly IProjectRepository _projectRepo = projectRepo;
        private readonly IStaffService _staffService = staffService;
        private readonly IServiceService _serviceService = serviceService;
        private readonly ICustomerService _customerService = customerService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory = dbContextFactory;

        // ✅ Get All Projects
        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await _projectRepo.GetAllAsync();

            return projects.Select(p => new ProjectDto
            {
                ProjectNumber = p.ProjectNumber ?? "N/A", // ✅ Default value for safety
                Name = p.Name ?? "Unnamed project", // ✅ Ensure project has a name
                StartDate = p.StartDate,  // ✅ Already a `DateTime`, so it's safe
                EndDate = p.EndDate,  // ✅ Already a `DateTime`, so it's safe
                CustomerName = p.Customer?.Name ?? "Ingen kund", // ✅ Prevents null reference
                ContactPerson = p.Customer?.ContactPerson ?? "Ingen kontaktperson", // ✅ Prevents null reference
                ServiceId = p.ServiceId ?? 0, // ✅ Ensure non-null value
                StaffId = p.StaffId ?? 0, // ✅ Ensure non-null value
                StatusId = p.StatusId ?? 0, // ✅ Ensure non-null value
                StatusName = p.Status?.Name ?? "Ingen status", // ✅ Prevents null reference
                TotalPrice = p.TotalPrice >= 0 ? p.TotalPrice : 0, // ✅ Prevents negative values
                Description = string.IsNullOrWhiteSpace(p.Description) ? "Ingen beskrivning" : p.Description, // ✅ Prevents null descriptions
                Service = p.Service != null
                    ? new ServiceDto { Name = p.Service.Name ?? "Unknown service", HourlyPrice = p.Service.HourlyPrice }
                    : new ServiceDto { Name = "No Service", HourlyPrice = 0 },
                Staff = p.Staff != null
                    ? new StaffDto { Name = p.Staff.Name ?? "Unknown staff", RoleName = p.Staff.Role?.Name ?? "Unknown role" }
                    : new StaffDto { Name = "No Staff", RoleName = "N/A" }
            }).ToList();
        }


        // ✅ Get Project by Number
        public async Task<ProjectDto?> GetProjectByNumberAsync(string projectNumber)
        {
            if (string.IsNullOrWhiteSpace(projectNumber))
                throw new ArgumentException("Project number is required.", nameof(projectNumber));

            var project = await _projectRepo.GetAsync(projectNumber);
            if (project == null) return null;  // ✅ Return null safely if project is not found

            return new ProjectDto
            {
                ProjectNumber = project.ProjectNumber ?? "N/A", // ✅ Default value
                Name = project.Name ?? "Unnamed Project", // ✅ Default value
                StartDate = project.StartDate,  // ✅ Already a `DateTime`, so it's safe
                EndDate = project.EndDate,  // ✅ Already a `DateTime`, so it's safe
                CustomerName = project.Customer?.Name ?? "Unknown Customer", // ✅ Prevents null reference
                ServiceId = project.ServiceId ?? 0,  // ✅ Ensure non-null value
                StaffId = project.StaffId ?? 0,  // ✅ Ensure non-null value
                StatusId = project.StatusId ?? 0,  // ✅ Ensure non-null value
                TotalPrice = project.TotalPrice >= 0 ? project.TotalPrice : 0, // ✅ Prevents negative values
                Description = string.IsNullOrWhiteSpace(project.Description) ? "No description provided" : project.Description // ✅ Prevents null descriptions
            };
        }


        // ✅ Get Project Statuses (Using `DbContextFactory` for Read-Only Query)
        public async Task<IEnumerable<StatusDto>> GetProjectStatusesAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var statuses = await dbContext.Statuses.ToListAsync();

            return statuses.Select(s => new StatusDto
            {
                StatusId = s.StatusId,
                Name = s.Name
            }).ToList();
        }

        // ✅ Create Project
        public async Task<string> CreateProjectAsync(ProjectDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // ✅ Ensure service is not null before passing it
                int serviceId = dto.Service != null
                    ? await _serviceService.EnsureServiceAsync(dto.Service)
                    : throw new ArgumentException("Service information is required.");

                // ✅ Ensure staff is not null before passing it
                int staffId = dto.Staff != null
                    ? await _staffService.EnsureStaffAsync(dto.Staff)
                    : throw new ArgumentException("Staff information is required.");

                var project = new Project
                {
                    ProjectNumber = dto.ProjectNumber,
                    Name = dto.Name,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    CustomerId = dto.CustomerId,
                    ServiceId = serviceId,
                    StaffId = staffId,
                    StatusId = dto.StatusId,
                    TotalPrice = dto.TotalPrice,
                    Description = dto.Description
                };

                await _projectRepo.AddAsync(project);
                await _unitOfWork.CommitAsync();
                return dto.ProjectNumber;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }


        // ✅ Create Project with Full Details (Using Transactions)
        public async Task<string> CreateProjectWithDetailsAsync(ProjectCreateDetailedDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Project details cannot be null.");

            if (string.IsNullOrWhiteSpace(dto.ProjectNumber))
                throw new ArgumentException("ProjectNumber is required.", nameof(dto.ProjectNumber));

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Project name is required.", nameof(dto.Name));

            if (dto.Service == null)
                throw new ArgumentNullException(nameof(dto.Service), "Service details are required.");

            if (dto.Staff == null)
                throw new ArgumentNullException(nameof(dto.Staff), "Staff details are required.");

            if (dto.StatusId <= 0)
                throw new ArgumentException("A valid Status ID is required.", nameof(dto.StatusId));

            if (dto.TotalPrice < 0)
                throw new ArgumentException("Total price cannot be negative.", nameof(dto.TotalPrice));

            if (!dto.StartDate.HasValue || dto.StartDate == null)
                throw new ArgumentException("StartDate is required.", nameof(dto.StartDate));

            if (!dto.EndDate.HasValue || dto.EndDate == null)
                throw new ArgumentException("EndDate is required.", nameof(dto.EndDate));

            Console.WriteLine($"🔵 [CreateProjectWithDetailsAsync] Creating project '{dto.ProjectNumber}'...");

            bool startedTransaction = false;
            if (!_unitOfWork.HasActiveTransaction)
            {
                await _unitOfWork.BeginTransactionAsync();
                startedTransaction = true;
            }

            try
            {
                int finalCustomerId = dto.CustomerId > 0
                    ? dto.CustomerId
                    : dto.Customer != null && !string.IsNullOrWhiteSpace(dto.Customer.Name)
                        ? await _customerService.EnsureCustomerAsync(dto.Customer.Name, dto.Customer.ContactPerson ?? "Unknown Contact")
                        : throw new ArgumentException("Either a valid CustomerId or Customer details are required.", nameof(dto.Customer));

                int serviceId = await _serviceService.EnsureServiceAsync(dto.Service);
                int staffId = await _staffService.EnsureStaffAsync(dto.Staff);

                var projectEntity = new Project
                {
                    ProjectNumber = dto.ProjectNumber,
                    Name = dto.Name,
                    StartDate = dto.StartDate.Value,
                    EndDate = dto.EndDate.Value,
                    CustomerId = finalCustomerId,
                    ServiceId = serviceId,
                    StaffId = staffId,
                    StatusId = dto.StatusId,
                    TotalPrice = dto.TotalPrice,
                    Description = dto.Description ?? "No description provided"
                };

                await _projectRepo.AddAsync(projectEntity);
                if (startedTransaction) await _unitOfWork.CommitAsync();

                Console.WriteLine($"✅ [CreateProjectWithDetailsAsync] Created project '{dto.ProjectNumber}' successfully.");
                return dto.ProjectNumber;
            }
            catch (Exception ex)
            {
                if (startedTransaction) await _unitOfWork.RollbackAsync();
                Console.WriteLine($"❌ [CreateProjectWithDetailsAsync] Failed to create project '{dto.ProjectNumber}'. Error: {ex.Message}");
                throw;
            }
        }



        // ✅ Update Project (Transaction-Safe)
        public async Task UpdateProjectAsync(ProjectDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Project details cannot be null.");

            var existing = await _projectRepo.GetAsync(dto.ProjectNumber);
            if (existing == null)
                throw new InvalidOperationException($"Project '{dto.ProjectNumber}' not found.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                existing.Name = dto.Name ?? "Unnamed Project"; // ✅ Default value for null names
                existing.StartDate = dto.StartDate;
                existing.EndDate = dto.EndDate;
                existing.CustomerId = dto.CustomerId;
                existing.ServiceId = dto.ServiceId;
                existing.StaffId = dto.StaffId;
                existing.StatusId = dto.StatusId;
                existing.TotalPrice = dto.TotalPrice;
                existing.Description = string.IsNullOrWhiteSpace(dto.Description) ? "No description provided" : dto.Description; // ✅ Prevents null description

                if (dto.Service != null)
                    existing.ServiceId = await _serviceService.EnsureServiceAsync(dto.Service);
                if (dto.Staff != null)
                    existing.StaffId = await _staffService.EnsureStaffAsync(dto.Staff);
                if (dto.Customer != null)
                    existing.CustomerId = await _customerService.EnsureCustomerAsync(
                        dto.Customer.Name ?? "Unknown Customer", // ✅ Default value for null customer names
                        dto.Customer.ContactPerson ?? "Unknown Contact" // ✅ Default value for null contactPerson
                    );

                await _projectRepo.UpdateAsync(existing);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }



        // ✅ Delete Project
        public async Task<bool> DeleteProjectAsync(string projectNumber)
        {
            Console.WriteLine($"🔍 [DeleteProjectAsync] Deleting project '{projectNumber}'...");

            bool startedTransaction = false;
            if (!_unitOfWork.HasActiveTransaction)
            {
                await _unitOfWork.BeginTransactionAsync();
                startedTransaction = true;
            }

            try
            {
                await _projectRepo.DeleteAsync(projectNumber);
                if (startedTransaction) await _unitOfWork.CommitAsync();

                Console.WriteLine($"✅ [DeleteProjectAsync] Deleted project '{projectNumber}' successfully.");
                return true;
            }
            catch (Exception ex)
            {
                if (startedTransaction) await _unitOfWork.RollbackAsync();
                Console.WriteLine($"❌ [DeleteProjectAsync] Failed to delete project '{projectNumber}'. Error: {ex.Message}");
                throw;
            }
        }
    }
}
