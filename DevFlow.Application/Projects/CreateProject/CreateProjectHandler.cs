using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using MediatR;

namespace DevFlow.Application.Projects.CreateProject
{
    public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, CreateProjectResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IProjectRepository _projectRepository;
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        public CreateProjectHandler(IWorkspaceAuthorizationService workspaceAuthorizationService,IWorkspaceRepository workspaceRepository,IUnitOfWork unitOfWork,IWorkspaceMemberRepository workspaceMemberRepository,ICurrentUserService currentUserService, IProjectRepository projectRepository)
        {
            _currentUserService = currentUserService;
            _projectRepository = projectRepository;
            _workspaceMemberRepository = workspaceMemberRepository;
            _unitOfWork = unitOfWork;
            _workspaceRepository = workspaceRepository;
            _workspaceAuthorizationService = workspaceAuthorizationService;
        }

        public async Task<CreateProjectResult> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var workspace = await _workspaceRepository.GetByIdAsync(request.WorkspaceId);
            if (workspace == null)
{
    throw new NotFoundException("Workspace does not exist");
}
            await _workspaceAuthorizationService.EnsureAdminOrOwnerAsync(request.WorkspaceId);
           
            var exist = await _projectRepository.ExistsInWorkspaceAsync(request.WorkspaceId, request.ProjectName);
            if (exist)
            {
                throw new ConflictException("Project already Exist in Workspace");
            }

            var project = new Project(
    request.ProjectName,
    request.Description,
    request.WorkspaceId,
    userId);
            
            await _projectRepository.AddAsync(project);
            await _unitOfWork.SaveChangesAsync();
            var result = new CreateProjectResult
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
            };
            return result;

        }
    }
}
