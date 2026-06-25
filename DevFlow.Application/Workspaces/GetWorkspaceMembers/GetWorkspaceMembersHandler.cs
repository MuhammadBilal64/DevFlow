using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Enum;
using MediatR;

namespace DevFlow.Application.Workspaces.GetWorkspaceMembers
{
    public class GetWorkspaceMembersHandler : IRequestHandler<GetWorkspaceMembersQuery,List<GetWorkspaceMembersResult>>
    {
        public readonly IWorkspaceRepository _workspaceRepository;
        public readonly IWorkspaceMemberRepository _workspaceMemberRepository;
        public readonly ICurrentUserService _currentUserService;
        public readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        public GetWorkspaceMembersHandler(IWorkspaceAuthorizationService workspaceAuthorizationService,IWorkspaceMemberRepository workspaceMemberRepository,IWorkspaceRepository workspaceRepository, ICurrentUserService currentUserService)
        {
            _workspaceRepository = workspaceRepository;
            _currentUserService = currentUserService;
            _workspaceMemberRepository = workspaceMemberRepository;
            _workspaceAuthorizationService = workspaceAuthorizationService;
        }

        public async Task<List<GetWorkspaceMembersResult>> Handle(GetWorkspaceMembersQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            await _workspaceAuthorizationService.EnsureAdminOrOwnerAsync(request.WorkspaceId);
            var members=await _workspaceMemberRepository.GetAllMembersAsync(request.WorkspaceId);
            var result = members.Select(x => new GetWorkspaceMembersResult
            {
                UserId = x.UserId,
                Name=x.User.Name,
                Role=x.Role,
                JoinedAt=x.JoinedAt,

            }
            ).ToList();
            return result;


            
        }
    }
}
