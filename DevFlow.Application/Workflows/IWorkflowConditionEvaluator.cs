using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Workflows
{
    public interface IWorkflowConditionEvaluator
    {
        bool Evaluate(
                WorkflowCondition condition,
                WorkflowExecutionContext context);
    }
}
