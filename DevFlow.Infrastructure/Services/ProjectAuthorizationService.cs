using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Enum;

namespace DevFlow.Infrastructure.Services
{
    public class ProjectAuthorizationService : IProjectAuthorizationService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        public ProjectAuthorizationService(
           ICurrentUserService currentUserService,
           IProjectRepository projectRepository,
           IProjectMemberRepository projectMemberRepository,
           IWorkspaceAuthorizationService workspaceAuthorizationService)
        {
            _currentUserService = currentUserService;
            _projectRepository = projectRepository;
            _projectMemberRepository = projectMemberRepository;
            _workspaceAuthorizationService = workspaceAuthorizationService;
        }

        public async Task EnsureCanManageProjectAsync(int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
                throw new NotFoundException("Project not found.");
            try
            {
                await _workspaceAuthorizationService
                    .EnsureAdminOrOwnerAsync(project.WorkspaceId);

                return;
            }
            catch (ForbiddenException)
            {
                // Not workspace Owner/Admin.
                // Fall through to project-level check.
            }
            var userId = _currentUserService.UserId;

            var member = await _projectMemberRepository
                .GetMemberAsync(projectId, userId);

            if (member == null || member.Role != ProjectRole.ProjectManager)
                throw new ForbiddenException(
                    "You do not have permission to manage this project.");



        }

        public async Task EnsureProjectMemberAsync(int projectId)
        {
            var userId = _currentUserService.UserId;

            var member = await _projectMemberRepository
                .GetMemberAsync(projectId, userId);

            if (member == null)
            {
                throw new ForbiddenException(
                    "You are not a member of this project.");
            }
        }

        public async Task EnsureProjectMemberAsync(int projectId, int userId)
        {
            var member = await _projectMemberRepository
        .GetMemberAsync(projectId, userId);
            if (member == null)
            {
                throw new NotFoundException(
                    "User is not a member of this project.");
            }

        }
    }
}
