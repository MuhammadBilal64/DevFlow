using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.GetAllWorkflows
{
    public class GetAllWorkflowsResult
    {

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public WorkflowTrigger Trigger { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
