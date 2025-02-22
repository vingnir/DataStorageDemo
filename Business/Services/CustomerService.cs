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
    public class CustomerService(
        ICustomerRepository customerRepo,
        IUnitOfWork unitOfWork,
        IDbContextFactory<AppDbContext> dbContextFactory
    ) : ICustomerService
    {
        private readonly ICustomerRepository _customerRepo = customerRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory = dbContextFactory;

        // ✅ Ensure Customer Exists (Only start transaction if needed)
        public async Task<int> EnsureCustomerAsync(string name, string? contactPerson)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name cannot be empty.");

            Console.WriteLine($"🔍 [EnsureCustomerAsync] Checking if customer '{name}' exists...");

            var existing = await _customerRepo.GetByNameAsync(name);
            if (existing != null)
            {
                Console.WriteLine($"✅ [EnsureCustomerAsync] Customer '{name}' exists (ID: {existing.CustomerId}).");
                return existing.CustomerId; // No transaction needed if found
            }

            Console.WriteLine($"❌ [EnsureCustomerAsync] Customer '{name}' not found, creating new one...");

            bool startedTransaction = false;
            if (!_unitOfWork.HasActiveTransaction)
            {
                await _unitOfWork.BeginTransactionAsync();
                Console.WriteLine("🔵 [EnsureCustomerAsync] Started new transaction.");
                startedTransaction = true;
            }

            try
            {
                var newCustomer = new Customer
                {
                    Name = name,
                    ContactPerson = contactPerson ?? "Unknown Contact" // ✅ Default value for null
                };

                await _customerRepo.AddAsync(newCustomer);
                if (startedTransaction)
                {
                    await _unitOfWork.CommitAsync();
                    Console.WriteLine($"✅ [EnsureCustomerAsync] Created Customer '{newCustomer.Name}' with ID {newCustomer.CustomerId}.");
                }

                return newCustomer.CustomerId;
            }
            catch
            {
                if (startedTransaction) await _unitOfWork.RollbackAsync();
                Console.WriteLine($"❌ [EnsureCustomerAsync] Failed to create customer '{name}'.");
                throw;
            }
        }

        // ✅ Create Customer (Transaction-Safe)
        public async Task CreateCustomerAsync(CustomerDto customerDto)
        {
            if (customerDto == null)
                throw new ArgumentNullException(nameof(customerDto), "Customer details cannot be null.");

            if (string.IsNullOrWhiteSpace(customerDto.Name))
                throw new ArgumentException("Customer name is required.");

            if (string.IsNullOrWhiteSpace(customerDto.ContactPerson))
                throw new ArgumentException("Customer contact person is required.");

            Console.WriteLine($"🟢 [CreateCustomerAsync] Creating new customer: {customerDto.Name}");

            bool startedTransaction = false;
            if (!_unitOfWork.HasActiveTransaction)
            {
                await _unitOfWork.BeginTransactionAsync();
                startedTransaction = true;
            }

            try
            {
                var customer = new Customer
                {
                    Name = customerDto.Name,
                    ContactPerson = customerDto.ContactPerson
                };

                await _customerRepo.AddAsync(customer);
                if (startedTransaction)
                {
                    await _unitOfWork.CommitAsync();
                    Console.WriteLine($"✅ [CreateCustomerAsync] Created Customer '{customer.Name}' with ID {customer.CustomerId}.");
                }
            }
            catch
            {
                if (startedTransaction) await _unitOfWork.RollbackAsync();
                Console.WriteLine($"❌ [CreateCustomerAsync] Failed to create customer '{customerDto.Name}'.");
                throw;
            }
        }

        // ✅ Check if Customer Exists
        public async Task<bool> CheckCustomerExistsAsync(int customerId)
        {
            Console.WriteLine($"🔍 [CheckCustomerExistsAsync] Checking if customer with ID {customerId} exists...");

            var customer = await _customerRepo.GetByIdAsync(customerId);
            if (customer != null)
            {
                Console.WriteLine($"✅ [CheckCustomerExistsAsync] Customer with ID {customerId} exists.");
                return true;
            }

            Console.WriteLine($"❌ [CheckCustomerExistsAsync] Customer with ID {customerId} does not exist.");
            return false;
        }

        // ✅ Get All Customers (Using Factory for Read-Only Query)
        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var customers = await dbContext.Customers.ToListAsync();

            return customers.Select(c => new CustomerDto
            {
                CustomerId = c.CustomerId,
                Name = c.Name,
                ContactPerson = c.ContactPerson
            }).ToList();
        }
    }
}
