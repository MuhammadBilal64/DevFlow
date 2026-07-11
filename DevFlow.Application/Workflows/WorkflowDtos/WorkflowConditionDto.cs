using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.WorkflowDtos
{
    public class WorkflowConditionDto
    {
        public string Field { get; set; } = null!;

        public WorkflowOperator Operator { get; set; }

        public string Value { get; set; } = null!;
    }
}
