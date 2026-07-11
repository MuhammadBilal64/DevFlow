using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.ConditionEvaluation
{
    public interface IConditionOperatorEvaluator
    {
        WorkflowOperator Operator {  get; }
        bool Evaluate(object?actualValue,string expectedValue);
    }
}
