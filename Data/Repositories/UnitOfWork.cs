using Data.Contexts;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class UnitOfWork : IUnitOfWork, IAsyncDisposable
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false; // ✅ Track disposal

        public bool HasActiveTransaction => _transaction != null; // ✅ Track active transactions

        public UnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeginTransactionAsync()
        {
            if (HasActiveTransaction)
            {
                Console.WriteLine("⚠️ [UnitOfWork] WARNING: Transaction already in progress!");
                throw new InvalidOperationException("Transaction is already in progress.");
            }

            _transaction = await _context.Database.BeginTransactionAsync();
            Console.WriteLine("✅ [UnitOfWork] Transaction started successfully.");
        }

        public async Task CommitAsync()
        {
            if (!HasActiveTransaction)
            {
                Console.WriteLine("⚠️ [UnitOfWork] WARNING: No active transaction to commit.");
                return;
            }

            try
            {
                await _context.SaveChangesAsync();
                await _transaction!.CommitAsync();
                Console.WriteLine("✅ [UnitOfWork] Transaction committed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [UnitOfWork] ERROR: Commit failed: {ex.Message}");
                await RollbackAsync();
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (!HasActiveTransaction)
            {
                Console.WriteLine("⚠️ [UnitOfWork] WARNING: No active transaction to roll back.");
                return;
            }

            try
            {
                await _transaction!.RollbackAsync();
                Console.WriteLine("⚠️ [UnitOfWork] Transaction rolled back.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [UnitOfWork] ERROR: Rollback failed: {ex.Message}");
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }


        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null; // ✅ Reset transaction
                Console.WriteLine("🗑️ [UnitOfWork] Transaction disposed.");
            }
        }

        // ✅ Synchronous Dispose (for normal garbage collection)
        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Dispose();
                _disposed = true;
                Console.WriteLine("🗑️ [UnitOfWork] Disposed.");
            }
        }

        // ✅ Asynchronous Dispose (for async cleanup)
        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                if (HasActiveTransaction)
                {
                    await DisposeTransactionAsync();
                }

                await _context.DisposeAsync();
                _disposed = true;
                Console.WriteLine("🗑️ [UnitOfWork] Disposed asynchronously.");
            }
        }
    }
}
