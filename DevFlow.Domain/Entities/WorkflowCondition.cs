using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Domain.Entities
{
    public class WorkflowCondition
    {
        public int Id { get; private set; }
        public int WorkflowId {  get; private set; }
        public Workflow workflow { get; private set; } = null!;
        public string Field { get; private set; } = null!;
        public WorkflowOperator Operator {  get; private set; }
        public string Value {  get; private set; } = null!;



    }
}
