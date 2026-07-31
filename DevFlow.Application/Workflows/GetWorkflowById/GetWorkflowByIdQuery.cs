using MediatR;

namespace DevFlow.Application.Workflows.GetWorkflowById
{
    public class GetWorkflowByIdQuery : IRequest<GetWorkflowByIdResult>
    {
        public int ProjectId { get; set; }
        public int WorkflowId { get; set; }
    }
}
