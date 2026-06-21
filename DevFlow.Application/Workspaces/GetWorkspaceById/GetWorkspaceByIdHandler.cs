using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Workspaces.GetWorkspaceById
{
    public class GetWorkspaceByIdHandler : IRequestHandler<GetWorkspaceByIdQuery, GetWorkspaceByIdResult>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
        public GetWorkspaceByIdHandler(ICurrentUserService currentUserService,IWorkspaceMemberRepository workspaceMemberRepository,IWorkspaceRepository workspaceRepository) { 
            _workspaceRepository = workspaceRepository;
            _currentUserService= currentUserService;
            _workspaceMemberRepository= workspaceMemberRepository;
        }
        public async Task<GetWorkspaceByIdResult> Handle(GetWorkspaceByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var membership = await _workspaceMemberRepository.GetMemberAsync(userId, request.WorkspaceId);
            if (membership == null)
            {
                throw new UnauthorizedException("Not a workspace member");

            }
            var workspace =  await _workspaceRepository.GetByIdAsync(request.WorkspaceId);
            if (workspace == null)
            {
                throw new NotFoundException("Workspace doesnt exist");
            }
            var result = new GetWorkspaceByIdResult
            {
                Id = workspace.Id,
                Name = workspace.Name,
                CreatedAt = workspace.CreatedAt
            };
            return result;

        }
    }
}
