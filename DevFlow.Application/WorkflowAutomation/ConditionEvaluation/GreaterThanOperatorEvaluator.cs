using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;
using FluentValidation.Validators;

namespace DevFlow.Application.Workflows.ConditionEvaluation
{
    public class GreaterThanOperatorEvaluator : IConditionOperatorEvaluator
    {
        private readonly IWorkflowValueConverter _workflowValueConverter;
        public GreaterThanOperatorEvaluator(IWorkflowValueConverter workflowValueConverter)
        {
            _workflowValueConverter = workflowValueConverter;
        }
        public WorkflowOperator Operator => WorkflowOperator.GreaterThan;
        public bool Evaluate(object? actualValue, string expectedValue)
        {
            if (actualValue is null)
            {
                throw new InvalidOperationException(
                    "Actual value cannot be null.");
            }
            if(actualValue is not IComparable comparable)
            {
                throw new InvalidOperationException($"Type:'{actualValue?.GetType().Name}' does not support comparison");

            }
            var convertedExpected = _workflowValueConverter.ConvertToActualType(actualValue,expectedValue);
            return comparable.CompareTo(convertedExpected) > 0;

        }
    }
}
