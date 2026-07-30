using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Workflows.EnableWorkflow
{
    public class EnableWorkflowHandler
        : IRequestHandler<EnableWorkflowCommand>
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IProjectAuthorizationService _projectAuthorizationService;
        private readonly IUnitOfWork _unitOfWork;

        public EnableWorkflowHandler(
            IWorkflowRepository workflowRepository, IProjectAuthorizationService projectAuthorizationService,
            IUnitOfWork unitOfWork)
        {
            _workflowRepository = workflowRepository;
            _unitOfWork = unitOfWork;
            _projectAuthorizationService = projectAuthorizationService;
        }

        public async Task Handle(
            EnableWorkflowCommand request,
            CancellationToken cancellationToken)
        {
            var workflow =
                await _workflowRepository.GetByIdAsync(request.WorkflowId);

            if (workflow == null)
                throw new NotFoundException("Workflow does not exist.");
            await _projectAuthorizationService
    .EnsureCanManageProjectAsync(workflow.ProjectId);

            workflow.Enable();

            await _workflowRepository.UpdateAsync(workflow);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}