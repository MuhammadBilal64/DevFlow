using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.ConditionEvaluation
{
    public class WorkflowConditionEvaluator : IWorkflowConditionEvaluator
    {
        private IReadOnlyDictionary<WorkflowOperator, IConditionOperatorEvaluator> _evaluators;
        public WorkflowConditionEvaluator(IEnumerable<IConditionOperatorEvaluator>evaluators)
        {
            _evaluators = evaluators.ToDictionary(x => x.Operator);
        }
        public bool Evaluate(WorkflowCondition condition, WorkflowExecutionContext context)
        {
            var actualValue = context.GetValue(condition.Field);
            if (!_evaluators.TryGetValue(
                    condition.Operator,
                    out var evaluator))
            {
                throw new InvalidOperationException(
                    $"No evaluator registered for operator '{condition.Operator}'.");
            }
            return evaluator.Evaluate(actualValue, condition.Value);


        }
    }
}
