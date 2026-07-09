using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.WorkflowDtos
{
    public class WorkflowActionDto
    {
        public WorkflowActionType ActionType { get; set; }

        public string Parameters { get; set; } = null!;

        public int Order { get; set; }
    }
}
