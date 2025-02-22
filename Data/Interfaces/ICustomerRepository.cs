using Data.Entities;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByNameAsync(string customerName);
        Task<Customer?> GetByIdAsync(int customerId); 
        // Task AddAsync(Customer customer);
    }
}
