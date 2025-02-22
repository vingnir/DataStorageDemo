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
    public class ServiceService(
        IServiceRepository serviceRepo,
        IUnitOfWork unitOfWork,  // ✅ Use UnitOfWork instead of DbContext
        IDbContextFactory<AppDbContext> dbContextFactory // ✅ Use Factory for Read-Only Queries
    ) : IServiceService
    {
        private readonly IServiceRepository _serviceRepo = serviceRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory = dbContextFactory;

        // ✅ Ensure Service Exists (Using Transactions)
        public async Task<int> EnsureServiceAsync(ServiceDto serviceDto)
        {
            if (serviceDto == null)
                throw new ArgumentException("Service details cannot be null.");

            if (string.IsNullOrWhiteSpace(serviceDto.Name))
                throw new ArgumentException("Service name is required.");

            Console.WriteLine("🟢 [EnsureServiceAsync] Checking if service exists...");

            // Look up service by name (Read-Only Query)
            using var dbContext = _dbContextFactory.CreateDbContext();
            var existingService = await dbContext.Services.FirstOrDefaultAsync(s => s.Name == serviceDto.Name);

            if (existingService != null)
            {
                Console.WriteLine($"✅ [EnsureServiceAsync] Found existing service: {existingService.Name}");

                // ✅ Only update the price if it has changed
                if (existingService.HourlyPrice != serviceDto.HourlyPrice)
                {
                    Console.WriteLine($"🔄 [EnsureServiceAsync] Updating price for {existingService.Name}...");

                    bool transactionStartedHere = false;
                    if (!_unitOfWork.HasActiveTransaction)
                    {
                        await _unitOfWork.BeginTransactionAsync();
                        Console.WriteLine("🔵 [EnsureServiceAsync] Started new transaction.");
                        transactionStartedHere = true;
                    }

                    try
                    {
                        await _serviceRepo.UpdateAsync(existingService);
                        if (transactionStartedHere)
                        {
                            await _unitOfWork.CommitAsync();
                            Console.WriteLine("🟢 [EnsureServiceAsync] Transaction committed.");
                        }
                    }
                    catch
                    {
                        if (transactionStartedHere)
                        {
                            await _unitOfWork.RollbackAsync();
                            Console.WriteLine("❌ [EnsureServiceAsync] Transaction rolled back due to an error.");
                        }
                        throw;
                    }
                }

                return existingService.ServiceId;
            }

            Console.WriteLine("❌ [EnsureServiceAsync] Service not found, creating new one...");

            bool startedTransaction = false;
            if (!_unitOfWork.HasActiveTransaction)
            {
                await _unitOfWork.BeginTransactionAsync();
                Console.WriteLine("🔵 [EnsureServiceAsync] Started new transaction.");
                startedTransaction = true;
            }

            try
            {
                var newService = new Service
                {
                    Name = serviceDto.Name,
                    HourlyPrice = serviceDto.HourlyPrice
                };

                await _serviceRepo.AddAsync(newService);
                if (startedTransaction)
                {
                    await _unitOfWork.CommitAsync();
                    Console.WriteLine("🟢 [EnsureServiceAsync] Transaction committed.");
                }

                Console.WriteLine($"✅ [EnsureServiceAsync] Created new service: {newService.Name}");
                return newService.ServiceId;
            }
            catch
            {
                if (startedTransaction)
                {
                    await _unitOfWork.RollbackAsync();
                    Console.WriteLine("❌ [EnsureServiceAsync] Transaction rolled back due to an error.");
                }
                throw;
            }
        }



        // ✅ Get All Services (Using Factory for Read-Only Query)
        public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();  // ✅ Use Factory for short-lived queries
            var services = await dbContext.Services.ToListAsync();

            return services.Select(s => new ServiceDto
            {
                ServiceId = s.ServiceId,
                Name = s.Name,
                HourlyPrice = s.HourlyPrice
            }).ToList();  // ✅ Convert to DTO
        }
    }
}
