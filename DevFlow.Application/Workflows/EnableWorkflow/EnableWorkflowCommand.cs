using MediatR;

namespace DevFlow.Application.Workflows.EnableWorkflow
{
    public class EnableWorkflowCommand : IRequest
    {
        public int ProjectId { get; set; }
        public int WorkflowId { get; set; }
    }
}
