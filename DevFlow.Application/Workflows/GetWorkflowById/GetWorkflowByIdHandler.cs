using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Workflows.WorkflowDtos;
using MediatR;

namespace DevFlow.Application.Workflows.GetWorkflowById
{
    public class GetWorkflowByIdHandler
        : IRequestHandler<GetWorkflowByIdQuery, GetWorkflowByIdResult>
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IProjectAuthorizationService _projectAuthorizationService;

        public GetWorkflowByIdHandler(
            IWorkflowRepository workflowRepository,
            IProjectAuthorizationService projectAuthorizationService)
        {
            _workflowRepository = workflowRepository;
            _projectAuthorizationService = projectAuthorizationService;
        }

        public async Task<GetWorkflowByIdResult> Handle(
            GetWorkflowByIdQuery request,
            CancellationToken cancellationToken)
        {
            var workflow =
                await _workflowRepository.GetByIdAsync(request.WorkflowId);

            if (workflow == null)
                throw new NotFoundException("Workflow does not exist.");

            await _projectAuthorizationService
                .EnsureProjectMemberAsync(workflow.ProjectId);

            return new GetWorkflowByIdResult
            {
                Id = workflow.Id,
                Name = workflow.Name,
                Description = workflow.Description,
                Trigger = workflow.Trigger,
                IsEnabled = workflow.IsEnabled,

                Conditions = workflow.Conditions
                    .Select(c => new WorkflowConditionDto
                    {
                        Field = c.Field,
                        Operator = c.Operator,
                        Value = c.Value
                    })
                    .ToList(),

                Actions = workflow.Actions
                    .OrderBy(a => a.Order)
                    .Select(a => new WorkflowActionDto
                    {
                        ActionType = a.ActionType,
                        Parameters = a.Parameters,
                        Order = a.Order
                    })
                    .ToList()
            };
        }
    }
}