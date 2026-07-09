using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.ConditionEvaluation
{
    public class LessThanOperatorEvaluator : IConditionOperatorEvaluator
    {
        public WorkflowOperator Operator => WorkflowOperator.LessThan;
        private readonly IWorkflowValueConverter _workflowValueConverter;
        public LessThanOperatorEvaluator(IWorkflowValueConverter workflowValueConverter)
        {
            _workflowValueConverter = workflowValueConverter;
        }
        public bool Evaluate(object? actualValue, string expectedValue)
        {
            var ConvertedExpected = _workflowValueConverter.ConvertToActualType(actualValue, expectedValue);
            if (actualValue is not IComparable comparable)
            {
                throw new InvalidOperationException($"Type:'{actualValue?.GetType().Name}' doesnot support comparison");

            }
            return comparable.CompareTo(ConvertedExpected) < 0;
        }
    }
}
