using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.GetAllWorkflows
{
    public class GetWorkflowsByProjectResult
    {

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public WorkflowTrigger Trigger { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
