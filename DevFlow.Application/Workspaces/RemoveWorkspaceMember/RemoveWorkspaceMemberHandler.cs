using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Enum;
using MediatR;

namespace DevFlow.Application.Workspaces.RemoveWorkspaceMember

{
    public class RemoveWorkspaceMemberHandler : IRequestHandler<RemoveWorkspaceMemberCommand, RemoveWorkspaceMemberResult>
    {
    private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
    private readonly ICurrentUserService _currentUserService;

        public RemoveWorkspaceMemberHandler(IWorkspaceMemberRepository workspaceMemberRepository,ICurrentUserService currentUserService)
        {
          _currentUserService= currentUserService;
          _workspaceMemberRepository = workspaceMemberRepository;  
        }
        public async Task<RemoveWorkspaceMemberResult> Handle(RemoveWorkspaceMemberCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var membership = await _workspaceMemberRepository.GetMemberAsync(userId,request.WorkspaceId);
            if (membership == null)
            {
                throw new UnauthorizedException("Not a workspace member");
            }
            if (membership.Role != WorkspaceRole.Owner && membership.Role != WorkspaceRole.Admin)
            {
                throw new UnauthorizedException("You dont have authority");
            }
            var member=await _workspaceMemberRepository.GetMemberAsync(request.UserId, request.WorkspaceId);
            if (member == null)
            {
                throw new NotFoundException("Target member not found");
            }
            if(member.Role == WorkspaceRole.Owner)
            {
                throw new ConflictException("Owner cannot be removed");
            }
            if (request.UserId == userId)
            {
                throw new ConflictException("You cannot remove yourself");
            }
            await _workspaceMemberRepository.RemoveAsync(request.UserId, request.WorkspaceId);
            return new RemoveWorkspaceMemberResult
            {
                Success = true
            };

        }
    }
}
