using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.CreateWorkflow
{
    public class CreateWorkflowActionDto
    {
        public WorkflowActionType ActionType { get; set; }

        public string Parameters { get; set; } = null!;

        public int Order { get; set; }
    }
}
