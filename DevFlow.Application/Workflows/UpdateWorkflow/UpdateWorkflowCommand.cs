using DevFlow.Application.Workflows.WorkflowDtos;
using MediatR;

namespace DevFlow.Application.Workflows.UpdateWorkflow
{
    public class UpdateWorkflowCommand : IRequest
    {
        public int WorkflowId { get; set; }
        public int ProjectId { get; set; }


        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public List<WorkflowConditionDto> Conditions { get; set; } = new();

        public List<WorkflowActionDto> Actions { get; set; } = new();
    }
}
