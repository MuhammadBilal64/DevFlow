using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.ConditionEvaluation
{
    public class LessThanOrEqualOperator : IConditionOperatorEvaluator
    {
        public WorkflowOperator Operator => WorkflowOperator.LessThanOrEqual;
        private readonly IWorkflowValueConverter _workflowValueConverter;
        public LessThanOrEqualOperator(IWorkflowValueConverter workflowValueConverter)
        {
            _workflowValueConverter = workflowValueConverter;
        }
        public bool Evaluate(object? actualValue, string expectedValue)
        {
            if (actualValue is null)
            {
                throw new InvalidOperationException(
                    "Actual value cannot be null.");
            }
            if (actualValue is not IComparable comparable)
            {
                throw new InvalidOperationException($"Type:'{actualValue?.GetType().Name}' does not support comparison");

            }
            var ConvertedExpected = _workflowValueConverter.ConvertToActualType(actualValue, expectedValue);
            return comparable.CompareTo(ConvertedExpected) <= 0;
        }
    }
}
