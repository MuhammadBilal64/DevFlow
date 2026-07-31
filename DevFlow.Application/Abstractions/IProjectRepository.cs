using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Abstractions
{
    public interface IProjectRepository
    {
        Task AddAsync(Project project);

        Task<Project?> GetByIdAsync(int Id);

        Task<bool> ExistsInWorkspaceAsync(int workspaceId, string projectName);

        Task<PaginatedData<Project>> GetProjectsForUserAsync(
            int workspaceId,
            int userId,
            string? searchTerm,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize);

        Task UpdateAsync(Project project);
    }
}