using Business.Dtos;
using Data.Entities;

namespace Business.Interfaces
{
    public interface IStaffService
    {
        Task<int> EnsureStaffAsync(StaffDto staffDto);
        Task<bool> CheckStaffExistsAsync(int staffId);
        Task<IEnumerable<StaffDto>> GetAllStaffAsync();
    }
}
