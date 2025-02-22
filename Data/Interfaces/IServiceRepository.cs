using Data.Entities;

namespace Data.Interfaces
{
    public interface IServiceRepository : IRepository<Service>
    {
        Task<Service?> GetByNameAsync(string serviceName);

    }
}
