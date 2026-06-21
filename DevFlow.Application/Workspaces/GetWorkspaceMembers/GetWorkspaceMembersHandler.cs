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
        public GetWorkspaceMembersHandler(IWorkspaceMemberRepository workspaceMemberRepository,IWorkspaceRepository workspaceRepository, ICurrentUserService currentUserService)
        {
            _workspaceRepository = workspaceRepository;
            _currentUserService = currentUserService;
            _workspaceMemberRepository = workspaceMemberRepository;
        }

        public async Task<List<GetWorkspaceMembersResult>> Handle(GetWorkspaceMembersQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var membership = await _workspaceMemberRepository.GetMemberAsync(userId, request.WorkspaceId);
            if (membership == null)
            {
                throw new UnauthorizedException("Not a workspace member");
            }
            if (membership.Role != WorkspaceRole.Owner && membership.Role != WorkspaceRole.Admin)
            {
                throw new UnauthorizedException("User dont have permission");
            }
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
