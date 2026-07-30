using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.ProjectMembers.RemoveProjectMember
{
    public class RemoveProjectMemberHandler
     : IRequestHandler<
         RemoveProjectMemberCommand,
         RemoveProjectMemberResult>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly IProjectAuthorizationService _projectAuthorizationService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveProjectMemberHandler(
            IProjectRepository projectRepository,
            IProjectMemberRepository projectMemberRepository,
            IProjectAuthorizationService projectAuthorizationService,
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _projectMemberRepository = projectMemberRepository;
            _projectAuthorizationService = projectAuthorizationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<RemoveProjectMemberResult> Handle(RemoveProjectMemberCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository
                .GetByIdAsync(request.ProjectId);

            if (project == null)
                throw new NotFoundException("Project not found.");

            await _projectAuthorizationService
                .EnsureCanManageProjectAsync(request.ProjectId);

            var member = await _projectMemberRepository
                .GetMemberAsync(
                    request.ProjectId,
                    request.UserId);

            if (member == null)
                throw new NotFoundException(
                    "Project member not found.");
            if (member.Role == Domain.Enum.ProjectRole.ProjectManager)
            {
                var managers = await _projectMemberRepository.GetAllByProjectIdAsync(request.ProjectId);
                if (managers.Count(m => m.Role == Domain.Enum.ProjectRole.ProjectManager) == 1)
                {
                    throw new ConflictException(
                        "A project must have at least one Project Manager.");
                }

            }
            await _projectMemberRepository.RemoveAsync(member);

            await _unitOfWork.SaveChangesAsync();

            return new RemoveProjectMemberResult
            {
                ProjectId = request.ProjectId,
                UserId = request.UserId
            };
        }
    }
}
