using Business.Dtos;

namespace Business.Interfaces
{
    public interface IRoleService
    {
        Task<int> EnsureRoleAsync(string roleName);
    }
}
