using Business.Dtos;
using Data.Entities;

namespace Business.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
        Task<ProjectDto?> GetProjectByNumberAsync(string projectNumber);
        Task<string> CreateProjectAsync(ProjectDto dto);
        Task<string> CreateProjectWithDetailsAsync(ProjectCreateDetailedDto dto);
        Task UpdateProjectAsync(ProjectDto dto);
        Task<bool> DeleteProjectAsync(string projectNumber);
        Task<IEnumerable<StatusDto>> GetProjectStatusesAsync();
    }
}
