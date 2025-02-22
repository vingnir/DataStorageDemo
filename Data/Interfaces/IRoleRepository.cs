using Data.Entities;

namespace Data.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role?> GetByNameAsync(string roleName);

        Task<Role?> GetRoleByIdAsync(int roleId);
    }
}
