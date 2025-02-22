using Business.Dtos;
using Data.Entities;

namespace Business.Interfaces
{
    public interface IServiceService
    {
        Task<int> EnsureServiceAsync(ServiceDto serviceDto);
        Task<IEnumerable<ServiceDto>> GetAllServicesAsync();
    }
}
