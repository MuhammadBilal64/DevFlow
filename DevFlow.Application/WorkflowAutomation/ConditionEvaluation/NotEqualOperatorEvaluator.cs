using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.ConditionEvaluation
{
    public class NotEqualOperatorEvaluator : IConditionOperatorEvaluator
    {
        private readonly IWorkflowValueConverter _valueConverter;
        public NotEqualOperatorEvaluator(IWorkflowValueConverter workflowValueConverter)
        {
            _valueConverter = workflowValueConverter;
        }

        public WorkflowOperator Operator => WorkflowOperator.NotEquals;

        public bool Evaluate(object? actualValue, string expectedValue)
        {
            if (actualValue is null)
            {
                throw new InvalidOperationException(
                    "Actual value cannot be null.");
            }
            var convertedExpected =
    _valueConverter.ConvertToActualType(actualValue, expectedValue);

            return !Equals(actualValue, convertedExpected);
        }
    }
}
