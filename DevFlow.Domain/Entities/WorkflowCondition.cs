using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Domain.Entities
{
    public class WorkflowCondition
    {
        public int Id { get; private set; }
        public int WorkflowId {  get; private set; }
       public Workflow Workflow { get; private set; } = null!;
        public string Field { get; private set; } = null!;
        public WorkflowOperator Operator {  get; private set; }
        public string Value {  get; private set; } = null!;
        public WorkflowCondition(string field,WorkflowOperator workflowOperator,string value)
        {


            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Field cannot be empty.", nameof(field));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be empty.", nameof(value));

            
            Field=field;
            Operator=workflowOperator;
            Value=value;
        }

        private WorkflowCondition()
        {
        }
    }
}
