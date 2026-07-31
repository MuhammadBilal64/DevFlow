using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.ProjectMembers.GetProjectMembers
{
    public class GetProjectMembersHandler
        : IRequestHandler<GetProjectMembersQuery, List<GetProjectMembersResult>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly IProjectAuthorizationService _projectAuthorizationService;

        public GetProjectMembersHandler(
            IProjectRepository projectRepository,
            IProjectMemberRepository projectMemberRepository,
            IProjectAuthorizationService projectAuthorizationService)
        {
            _projectRepository = projectRepository;
            _projectMemberRepository = projectMemberRepository;
            _projectAuthorizationService = projectAuthorizationService;
        }

        public async Task<List<GetProjectMembersResult>> Handle(
            GetProjectMembersQuery request,
            CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);

            if (project == null)
                throw new NotFoundException("Project not found.");

            await _projectAuthorizationService
                .EnsureProjectMemberAsync(request.ProjectId);

            var members = await _projectMemberRepository
                .GetAllByProjectIdAsync(request.ProjectId);

            return members
                .Select(m => new GetProjectMembersResult
                {
                    UserId = m.UserId,
                    Name = m.User.Name,
                    Email = m.User.Email,
                    Role = m.Role.ToString(),
                    JoinedAt = m.JoinedAt
                })
                .ToList();
        }
    }
}