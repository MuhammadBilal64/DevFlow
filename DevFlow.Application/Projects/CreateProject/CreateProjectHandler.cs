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
        public CreateProjectHandler(IWorkspaceRepository workspaceRepository,IUnitOfWork unitOfWork,IWorkspaceMemberRepository workspaceMemberRepository,ICurrentUserService currentUserService, IProjectRepository projectRepository)
        {
            _currentUserService = currentUserService;
            _projectRepository = projectRepository;
            _workspaceMemberRepository = workspaceMemberRepository;
            _unitOfWork = unitOfWork;
            _workspaceRepository = workspaceRepository;
        }

        public async Task<CreateProjectResult> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var workspace = await _workspaceRepository.GetByIdAsync(request.WorkspaceId);
            if (workspace == null)
            {
                throw new NotFoundException("Workspace Doesnot Exist");
            }
            var membership =await _workspaceMemberRepository.GetMemberAsync(userId, request.WorkspaceId);
            if (membership == null)
            {
                throw new UnauthorizedException("Not a workspace member");
            }

            if (membership.Role != WorkspaceRole.Admin && membership.Role != WorkspaceRole.Owner)
            {
                throw new UnauthorizedException("Not allowed to Create Project");
            }
            var exist = await _projectRepository.ExistsInWorkspaceAsync(request.WorkspaceId, request.ProjectName);
            if (exist)
            {
                throw new ConflictException("Project already Exist in Workspace");
            }
            var project = new Project
            {
                Name = request.ProjectName,
                Description = request.Description,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                WorkspaceId = request.WorkspaceId,
            };
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
