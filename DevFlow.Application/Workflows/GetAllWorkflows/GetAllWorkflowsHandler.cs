using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using MediatR;

namespace DevFlow.Application.Workflows.GetAllWorkflows
{
    public class GetAllWorkflowHandler
        : IRequestHandler<GetAllWorkflowsQuery, PagedResult<GetAllWorkflowsResult>>
    {
        private readonly IWorkflowRepository _workflowRepository;

        public GetAllWorkflowHandler(IWorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }

        public async Task<PagedResult<GetAllWorkflowsResult>> Handle(
            GetAllWorkflowsQuery request,
            CancellationToken cancellationToken)
        {
            var data = await _workflowRepository.GetAllAsync(
                request.SearchTerm,
                request.Trigger,
                request.IsEnabled,
                request.SortBy,
                request.Descending,
                request.PageNumber,
                request.PageSize);

            return new PagedResult<GetAllWorkflowsResult>
            {
                Items = data.Items.Select(w => new GetAllWorkflowsResult
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