using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Projects.UpdateProject
{
    public class UpdateProjectHandler : IRequestHandler<UpdateProjectCommand, UpdateProjectResult>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectAuthorizationService _projectAuthorizationService;
        public UpdateProjectHandler(
      IProjectAuthorizationService projectAuthorizationService,
      IUnitOfWork unitOfWork,
      IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
            _projectAuthorizationService = projectAuthorizationService;
            _unitOfWork = unitOfWork;
        }
        public async Task<UpdateProjectResult> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {

            var project = await _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null)
            {
                throw new NotFoundException("No such Project Exist");
            }
            await _projectAuthorizationService
                .EnsureCanManageProjectAsync(project.Id);
            var exist = await _projectRepository.ExistsInWorkspaceAsync(project.WorkspaceId, request.Name);
            if (exist && request.Name != project.Name)
            {
                throw new ConflictException(
        "Project already exists in workspace");
            }
            project.UpdateName(request.Name);
            project.UpdateDescription(request.Description);

            await _projectRepository.UpdateAsync(project);
            await _unitOfWork.SaveChangesAsync();
            var result = new UpdateProjectResult
            {
                Id = project.Id,
                ProjectName = project.Name,
                Description = project.Description,
            };
            return result;

        }
    }
}
