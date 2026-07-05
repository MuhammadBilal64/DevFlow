using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Domain.Entities
{
    public class Workflow
    {
        public int Id { get;private set;  }
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public WorkflowTrigger Trigger {  get; private set; } 
        public bool IsEnabled {  get; private set; }=false;
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt {  get; private set; }
        public ICollection<WorkflowCondition>Conditions=new List<WorkflowCondition>();
        public ICollection<WorkflowAction> Actions=new List<WorkflowAction>();

    }
}
