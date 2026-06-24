using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Entities;
using MediatR;

namespace DevFlow.Application.Projects.GetProjectsByWorkspace
{
    public class GetProjectsByWorkspaceHandler : IRequestHandler<GetProjectsByWorkspaceQuery, List<GetProjectsByWorkspaceResult>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IProjectRepository _projectRepository;
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
        public GetProjectsByWorkspaceHandler(ICurrentUserService currentUserService,IProjectRepository projectRepository,IWorkspaceMemberRepository workspaceMemberRepository)
        {
            _currentUserService = currentUserService;
            _projectRepository= projectRepository;
            _workspaceMemberRepository= workspaceMemberRepository;
        }
        public async Task<List<GetProjectsByWorkspaceResult>> Handle(GetProjectsByWorkspaceQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var membership = await _workspaceMemberRepository.GetMemberAsync(userId, request.WorkspaceId);
            if (membership == null)
            {
                    throw new UnauthorizedException("Not a workspace member");

            }
            var result = await _projectRepository.GetProjectsByWorkspaceAsync(request.WorkspaceId);
            var projects = result.Select(x => new GetProjectsByWorkspaceResult
            {
                ProjectId = x.Id,
                ProjectName=x.Name,
                Description=x.Description,
            }).ToList();
            return projects;
        }
    }
}
