using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Workflows.CreateWorkflow;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.GetWorkflowById
{
    public class GetWorkflowByIdResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; public string? Description { get; set; }
        public WorkflowTrigger Trigger { get; set; }
        public bool IsEnabled { get; set; }
        public List<CreateWorkflowConditionDto> Conditions { get; set; } = new();
        public List<CreateWorkflowActionDto> Actions { get; set; } = new();
    }
}
