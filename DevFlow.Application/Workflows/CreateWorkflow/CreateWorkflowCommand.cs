using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Workflows.WorkflowDtos;
using DevFlow.Domain.Enum;
using MediatR;

namespace DevFlow.Application.Workflows.CreateWorkflow
{
    public class CreateWorkflowCommand:IRequest<CreateWorkflowResult>
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public WorkflowTrigger Trigger { get; set; }

        public List<WorkflowConditionDto> Conditions { get; set; } = new();

        public List<WorkflowActionDto> Actions { get; set; } = new();
    }
}
