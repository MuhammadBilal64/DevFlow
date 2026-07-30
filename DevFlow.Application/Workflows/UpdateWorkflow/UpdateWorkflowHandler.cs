using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Entities;
using MediatR;

namespace DevFlow.Application.Workflows.UpdateWorkflow
{
    public class UpdateWorkflowHandler
    : IRequestHandler<UpdateWorkflowCommand>
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectAuthorizationService _projectAuthorizationService;

        public UpdateWorkflowHandler(
            IWorkflowRepository workflowRepository, IProjectAuthorizationService projectAuthorizationService,
            IUnitOfWork unitOfWork)
        {
            _workflowRepository = workflowRepository;
            _unitOfWork = unitOfWork;
            _projectAuthorizationService = projectAuthorizationService;
        }
        public async Task Handle(UpdateWorkflowCommand request, CancellationToken cancellationToken)
        {
            var workflow =
            await _workflowRepository.GetByIdAsync(request.WorkflowId);

            if (workflow == null)
                throw new NotFoundException("Workflow does not exist.");
            await _projectAuthorizationService
    .EnsureCanManageProjectAsync(workflow.ProjectId);

            workflow.Update(
                request.Name,
                request.Description);

            foreach (var command in workflow.Actions.ToList())
            {
                workflow.RemoveAction(command);
            }
            foreach (var command in workflow.Conditions.ToList())
            {
                workflow.RemoveCondition(command);
            }
            foreach (var dto in request.Conditions)
            {
                workflow.AddCondition(
                    new WorkflowCondition(
                        dto.Field,
                        dto.Operator,
                        dto.Value));
            }
            foreach (var dto in request.Actions)
            {
                workflow.AddAction(
                    new WorkflowAction(
                        dto.ActionType,
                        dto.Parameters,
                        dto.Order));
            }
            if (!workflow.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await _workflowRepository.ExistsInProjectAsync(
                    workflow.ProjectId,
                    request.Name);

                if (exists)
                    throw new ConflictException(
                        "Workflow with this name already exists.");
            }
            await _workflowRepository.UpdateAsync(workflow);

            await _unitOfWork.SaveChangesAsync();

        }
    }
}
