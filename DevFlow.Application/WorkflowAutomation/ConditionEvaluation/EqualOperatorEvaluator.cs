using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.ConditionEvaluation
{
    public class EqualOperatorEvaluator : IConditionOperatorEvaluator
    {
        private readonly IWorkflowValueConverter _valueConverter;
        public EqualOperatorEvaluator(IWorkflowValueConverter workflowValueConverter)
        {
            _valueConverter = workflowValueConverter;
        }

        public WorkflowOperator Operator => WorkflowOperator.Equals;

        public bool Evaluate(object? actualValue, string expectedValue)
        {
            var convertedExpected =
    _valueConverter.ConvertToActualType(actualValue,expectedValue);

            return Equals(actualValue,convertedExpected);
        }
    }
}
