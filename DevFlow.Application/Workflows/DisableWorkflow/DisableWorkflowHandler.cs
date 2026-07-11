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

        public DisableWorkflowHandler(
            IWorkflowRepository workflowRepository,
            IUnitOfWork unitOfWork)
        {
            _workflowRepository = workflowRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            DisableWorkflowCommand request,
            CancellationToken cancellationToken)
        {
            var workflow =
                await _workflowRepository.GetByIdAsync(request.WorkflowId);

            if (workflow == null)
                throw new NotFoundException("Workflow does not exist.");

            workflow.Disable();

            await _workflowRepository.UpdateAsync(workflow);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}