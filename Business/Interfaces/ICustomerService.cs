using System.Threading.Tasks;
using Business.Dtos;
using Data.Entities;

namespace Business.Interfaces
{
    public interface ICustomerService
    {
        Task<int> EnsureCustomerAsync(string name, string contactPerson);
        Task CreateCustomerAsync(CustomerDto customer); 
        Task<bool> CheckCustomerExistsAsync(int customerId);

        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    }
}
