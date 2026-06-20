using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Abstractions
{
    public interface IWorkspaceMemberRepository
    {
        Task AddAsync(WorkspaceMember member);
        Task<List<WorkspaceMember>> GetByUserIdAsync(int userId);
        Task<WorkspaceMember> GetMemberAsync(int userId,int workspaceId);
    }
}
