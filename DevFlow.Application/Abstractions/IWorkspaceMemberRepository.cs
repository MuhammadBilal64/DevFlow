using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Abstractions
{
    public interface IWorkspaceMemberRepository
    {
        Task AddAsync(WorkspaceMember member);
        Task<List<WorkspaceMember>> GetByUserIdAsync(int userId);
        Task<PaginatedData<WorkspaceMember>> GetByUserIdAsync(
    int userId,
    int pageNumber,
    int pageSize);
        Task<WorkspaceMember?> GetMemberAsync(int userId,int workspaceId);
         Task<PagedResult<WorkspaceMember>> GetAllMembersAsync(int workspaceId, int pageNumber,
    int pageSize);
        Task RemoveAsync(int userId,int WorkspaceId);


    }
}
