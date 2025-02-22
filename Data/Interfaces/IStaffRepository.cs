using Data.Entities;

namespace Data.Interfaces
{
    public interface IStaffRepository : IRepository<Staff>
    {
        Task<Staff?> GetByNameAndRoleIdAsync(string staffName, int roleId); 
    }

}
