using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Tasks.GetTasksByProject
{
    public class GetTasksByProjectHandler : IRequestHandler<GetTasksByProjectQuery, PagedResult<GetTasksByProjectResult>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectAuthorizationService _projectAuthorizationService;
        public GetTasksByProjectHandler(
     IProjectRepository projectRepository,
     ITaskRepository taskRepository,
     IProjectAuthorizationService projectAuthorizationService)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _projectAuthorizationService = projectAuthorizationService;
        }

        public async Task<PagedResult<GetTasksByProjectResult>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null)
            {
                throw new NotFoundException("Project Doesnot Exist");
            }
            await _projectAuthorizationService
     .EnsureProjectMemberAsync(project.Id);

            var paginatedData = await _taskRepository.GetTasksByProjectAsync(
     request.ProjectId,
     request.SearchTerm,
     request.SortBy, request.Descending,
     request.Status, request.Priority,
     request.PageNumber,
     request.PageSize);
            var totalPages = (int)Math.Ceiling((double)paginatedData.TotalCount / request.PageSize);


            var tasks = paginatedData.Items.Select(i => new GetTasksByProjectResult
            {
                TaskId = i.Id,
                Title = i.Title,

            }).ToList();
            return new PagedResult<GetTasksByProjectResult>
            {
                Items = tasks,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = paginatedData.TotalCount,
                TotalPages = totalPages
            };


        }

    }
}
