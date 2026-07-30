using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Projects.GetProjectById
{
    public class GetProjectByIdHandler
        : IRequestHandler<GetProjectByIdQuery, GetProjectByIdResult>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectAuthorizationService _projectAuthorizationService;

        public GetProjectByIdHandler(
            IProjectRepository projectRepository,
            IProjectAuthorizationService projectAuthorizationService)
        {
            _projectRepository = projectRepository;
            _projectAuthorizationService = projectAuthorizationService;
        }

        public async Task<GetProjectByIdResult> Handle(
            GetProjectByIdQuery request,
            CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.Id);

            if (project == null)
                throw new NotFoundException("Project not found.");

            await _projectAuthorizationService
                .EnsureProjectMemberAsync(request.Id);

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