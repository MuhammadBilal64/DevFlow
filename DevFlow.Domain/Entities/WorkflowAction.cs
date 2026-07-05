using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Domain.Entities
{
    public class WorkflowAction
    {
        public int Id { get;private set }
        public int WorkflowId {  get; private set; }
        public WorkflowActionType ActionType { get; private set; }
        public string Parameters { get; private set } = null!;
        public int Order {  get; private set; }
    }
}
