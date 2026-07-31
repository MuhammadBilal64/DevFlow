using MediatR;

namespace DevFlow.Application.Workflows.DisableWorkflow
{
    public class DisableWorkflowCommand : IRequest
    {
        public int ProjectId { get; set; }
        public int WorkflowId { get; set; }

    }
}
