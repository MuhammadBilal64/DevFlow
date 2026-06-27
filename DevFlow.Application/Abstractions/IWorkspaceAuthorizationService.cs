using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Abstractions
{
    public interface IWorkspaceAuthorizationService
    {
        Task EnsureAdminOrOwnerAsync(int workspaceId);
        Task EnsureWorkspaceMemberAsync(int workspaceId,int userId);
        Task EnsureWorkspaceMemberAsync(int workspaceId);

    }
}
