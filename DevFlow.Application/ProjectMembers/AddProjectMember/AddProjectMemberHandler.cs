using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Entities;
using MediatR;

namespace DevFlow.Application.ProjectMembers.AddProjectMember
{
    public class AddProjectMemberHandler : IRequestHandler<AddProjectMemberCommand, AddProjectMemberResult>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectAuthorizationService _projectAuthorizationService;
        public AddProjectMemberHandler(IWorkspaceAuthorizationService workspaceAuthorizationService, IProjectRepository projectRepository, IUserRepository userRepository, IProjectMemberRepository projectMemberRepository, IProjectAuthorizationService projectAuthorizationService, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _projectMemberRepository = projectMemberRepository;
            _workspaceAuthorizationService = workspaceAuthorizationService;
            _unitOfWork = unitOfWork;
            _projectAuthorizationService = projectAuthorizationService;


        }

        public async Task<AddProjectMemberResult> Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
        {
            // 1. Project
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null)
                throw new NotFoundException("Project not found.");
            // 2. User
            var user = await _userRepository.GetByUserIdAsync(request.UserId);

            if (user == null)
                throw new NotFoundException("User not found.");
            // 3. Workspace membership
            await _workspaceAuthorizationService
                .EnsureWorkspaceMemberAsync(
                    project.WorkspaceId,
                    request.UserId);
            // 4. Current user permission
            await _projectAuthorizationService
                .EnsureCanManageProjectAsync(request.ProjectId);

            // 5. Already project member?
            var exists = await _projectMemberRepository
                .IsMemberAsync(request.ProjectId, request.UserId);

            if (exists)
            {
                throw new ConflictException(
                    "User is already a project member.");
            }
            // 6. Create member
            var member = new ProjectMember(
                project,
                request.UserId,
                request.Role);

            await _projectMemberRepository.AddAsync(member);

            await _unitOfWork.SaveChangesAsync();

            return new AddProjectMemberResult
            {
                ProjectId = project.Id,
                UserId = request.UserId
            };

        }
    }
}
