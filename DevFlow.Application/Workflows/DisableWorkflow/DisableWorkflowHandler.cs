using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Workflows.DisableWorkflow
{
    public class DisableWorkflowHandler
        : IRequestHandler<DisableWorkflowCommand>
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectAuthorizationService _projectAuthorizationService;
        public DisableWorkflowHandler(
            IWorkflowRepository workflowRepository, IProjectAuthorizationService projectAuthorizationService,
            IUnitOfWork unitOfWork)
        {
            _workflowRepository = workflowRepository;
            _unitOfWork = unitOfWork;
            _projectAuthorizationService = projectAuthorizationService;
        }

        public async Task Handle(
            DisableWorkflowCommand request,
            CancellationToken cancellationToken)
        {
            var workflow =
                await _workflowRepository.GetByIdAsync(request.WorkflowId);

            if (workflow == null)
                throw new NotFoundException("Workflow does not exist.");
            await _projectAuthorizationService
             .EnsureCanManageProjectAsync(workflow.ProjectId);

            workflow.Disable();

            await _workflowRepository.UpdateAsync(workflow);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}