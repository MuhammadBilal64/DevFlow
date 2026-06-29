using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Abstractions
{
    public interface IProjectRepository
    {
        Task AddAsync(Project project);
        Task<Project?> GetByIdAsync(int Id);
        Task<bool> ExistsInWorkspaceAsync(int workspaceId, string projectName);

        Task<PaginatedData<Project>>GetProjectsByWorkspaceAsync(int workspaceId,string ?SearchTerm,int pageNumber,
            int pageSize);
        Task UpdateAsync(Project project);

    }
}
