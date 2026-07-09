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
           var convertedExpected = _workflowValueConverter.ConvertToActualType(actualValue,expectedValue);
            if(actualValue is not IComparable comparable)
            {
                throw new InvalidOperationException($"Type:'{actualValue?.GetType().Name}' doesnot support comparison");

            }
            return comparable.CompareTo(convertedExpected) > 0;

        }
    }
}
