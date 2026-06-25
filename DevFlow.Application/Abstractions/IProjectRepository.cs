using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Abstractions
{
    public interface IProjectRepository
    {
        Task AddAsync(Project project);
        Task<Project?> GetByIdAsync(int Id);
        Task<bool> ExistsInWorkspaceAsync(int workspaceId, string projectName);

        Task<List<Project>>GetProjectsByWorkspaceAsync(int workspaceId);
        Task UpdateAsync(Project project);

    }
}
