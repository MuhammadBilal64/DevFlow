using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Domain.Entities
{
    public class WorkflowAction
    {
        public int Id { get; private set; }
        public int WorkflowId {  get; private set; }
        public Workflow Workflow { get; private set; } = null!;
        public WorkflowActionType ActionType { get; private set; }
        public string Parameters { get; private set; } = null!;
        public int Order {  get; private set; }
        public WorkflowAction(WorkflowActionType actionType,string Parameters_,int order)
        {
            
            if (string.IsNullOrWhiteSpace(Parameters_))
                throw new ArgumentException("Parameters cannot be empty",nameof(Parameters_));
            if (order <= 0)
                throw new ArgumentOutOfRangeException(nameof(order), "Order must be greater than zero.");


            ActionType=actionType;
            Parameters=Parameters_;
            Order = order;

        }
        private WorkflowAction()
        {

        }
    }
}
