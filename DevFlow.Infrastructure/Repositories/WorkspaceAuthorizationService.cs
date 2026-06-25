using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Enum;

namespace DevFlow.Infrastructure.Repositories
{
    public class WorkspaceAuthorizationService : IWorkspaceAuthorizationService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;

        public WorkspaceAuthorizationService(
            ICurrentUserService currentUserService,
            IWorkspaceMemberRepository workspaceMemberRepository)
        {
            _currentUserService = currentUserService;
            _workspaceMemberRepository = workspaceMemberRepository;
        }

  

        public async Task EnsureAdminOrOwnerAsync(int workspaceId)
        {
            var membership =
                await _workspaceMemberRepository.GetMemberAsync(
                    _currentUserService.UserId,
                    workspaceId);

            if (membership == null)
                throw new UnauthorizedException("Not a workspace member");

            if (membership.Role != WorkspaceRole.Owner &&
                membership.Role != WorkspaceRole.Admin)
            {
                throw new ForbiddenException(
                    "User does not have permission");
            }
        }


    }
}
