using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Projects.GetProjectById
{
    public class GetProjectByIdHandler : IRequestHandler<GetProjectByIdQuery, GetProjectByIdResult>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;

        public GetProjectByIdHandler(IProjectRepository projectRepository,ICurrentUserService currentUserService, IWorkspaceMemberRepository workspaceMemberRepository)
        {
            _projectRepository = projectRepository;
            _currentUserService = currentUserService;
            _workspaceMemberRepository = workspaceMemberRepository;
        } 
        public async Task<GetProjectByIdResult> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var project = await _projectRepository.GetByIdAsync(request.Id);
            if (project == null)
            {
                throw new NotFoundException("Project Not Found");
            }
            var membership = await _workspaceMemberRepository.GetMemberAsync(userId, project.WorkspaceId);
            if (membership == null)
            {
                throw new UnauthorizedException("Not a workspace member");
            }
            return new GetProjectByIdResult
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt
            };
        }
    }
}
