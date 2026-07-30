using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Workflows.GetAllWorkflows
{
    public class GetAllWorkflowHandler
        : IRequestHandler<GetWorkflowsByProjectQuery, PagedResult<GetWorkflowsByProjectResult>>
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectAuthorizationService _projectAuthorizationService;

        public GetAllWorkflowHandler(
            IWorkflowRepository workflowRepository,
            IProjectRepository projectRepository,
            IProjectAuthorizationService projectAuthorizationService)
        {
            _workflowRepository = workflowRepository;
            _projectRepository = projectRepository;
            _projectAuthorizationService = projectAuthorizationService;
        }

        public async Task<PagedResult<GetWorkflowsByProjectResult>> Handle(
            GetWorkflowsByProjectQuery request,
            CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);

            if (project == null)
                throw new NotFoundException("Project not found.");
            await _projectAuthorizationService
                .EnsureProjectMemberAsync(request.ProjectId);

            var data = await _workflowRepository.GetByProjectAsync(
                request.ProjectId,
                request.SearchTerm,
                request.Trigger,
                request.IsEnabled,
                request.SortBy,
                request.Descending,
                request.PageNumber,
                request.PageSize);

            return new PagedResult<GetWorkflowsByProjectResult>
            {
                Items = data.Items.Select(w => new GetWorkflowsByProjectResult
                {
                    Id = w.Id,
                    Name = w.Name,
                    Description = w.Description,
                    Trigger = w.Trigger,
                    IsEnabled = w.IsEnabled,
                    CreatedAt = w.CreatedAt
                }).ToList(),

                TotalCount = data.TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(
                    (double)data.TotalCount / request.PageSize)
            };
        }
    }
}