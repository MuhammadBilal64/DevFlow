using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.CreateWorkflow
{
    public class CreateWorkflowResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public WorkflowTrigger Trigger { get; set; }
    }
}
