using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.ConditionEvaluation
{
    public class ContainsOperatorEvaluator : IConditionOperatorEvaluator
    {
        public WorkflowOperator Operator => WorkflowOperator.Contains;
        private readonly IWorkflowValueConverter _workflowValueConverter;
        public ContainsOperatorEvaluator(IWorkflowValueConverter workflowValueConverter)
        {
            _workflowValueConverter = workflowValueConverter;
        }

        public bool Evaluate(object? actualValue, string expectedValue)
        {
            if (actualValue == null)
            {
                return false;
            }
            return actualValue.ToString()!.Contains(expectedValue, StringComparison.OrdinalIgnoreCase);
        }
    }
}
