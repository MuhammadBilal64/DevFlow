using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;
using MediatR;

namespace DevFlow.Application.Workflows.CreateWorkflow
{
    public class CreateWorkflowCommand:IRequest<CreateWorkflowResult>
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public WorkflowTrigger Trigger { get; set; }

        public List<CreateWorkflowConditionDto> Conditions { get; set; } = new();

        public List<CreateWorkflowActionDto> Actions { get; set; } = new();
    }
}
