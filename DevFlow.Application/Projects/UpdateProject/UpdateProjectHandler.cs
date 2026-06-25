using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using MediatR;

namespace DevFlow.Application.Projects.UpdateProject
{
    public class UpdateProjectHandler : IRequestHandler<UpdateProjectCommand, UpdateProjectResult>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateProjectHandler(IWorkspaceAuthorizationService workspaceAuthorizationService,IUnitOfWork unitOfWork ,IProjectRepository projectRepository,IWorkspaceMemberRepository workspaceMemberRepository,ICurrentUserService currentUserService)
        {
            _projectRepository = projectRepository;
            _workspaceMemberRepository=workspaceMemberRepository;
            _currentUserService=currentUserService;
            _workspaceAuthorizationService=workspaceAuthorizationService;
            _unitOfWork=unitOfWork;

        }
        public async Task<UpdateProjectResult> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null)
            {
                throw new NotFoundException("No such Project Exist");
            }
            await _workspaceAuthorizationService.EnsureAdminOrOwnerAsync(project.WorkspaceId);
            project.Name = request.Name;
            project.Description = request.Description;

            await _projectRepository.UpdateAsync(project);
           await _unitOfWork.SaveChangesAsync();
            var result = new UpdateProjectResult
            {
                Id=project.Id,
                ProjectName=project.Name,
                Description = project.Description,
};
            return result;

        }
    }
}
