using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Workflows.ActionExecution;
using DevFlow.Application.Workflows.ConditionEvaluation;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows
{
    public class WorkflowEngine : IWorkflowEngine
    {
        private readonly IWorkflowRepository _workflowRepository;

        private readonly IWorkflowConditionEvaluator _conditionEvaluator;

        private readonly IActionDispatcher _actionDispatcher;
        public WorkflowEngine(IWorkflowRepository workflowRepository, IWorkflowConditionEvaluator conditionEvaluator, IActionDispatcher actionDispatcher)
        {
            _workflowRepository = workflowRepository;
            _conditionEvaluator = conditionEvaluator;
            _actionDispatcher = actionDispatcher;
        }


        public async Task ExecuteAsync(WorkflowTrigger trigger, WorkflowExecutionContext workflowExecutionContext)
        {
            var workflows = await _workflowRepository.GetActiveByTriggerAsync(trigger);

            foreach (var workflow in workflows)
            {
                try
                {
                    bool passed = workflow.Conditions.All(condition =>
                        _conditionEvaluator.Evaluate(
                            condition,
                            workflowExecutionContext));

                    if (!passed)
                    {
                        continue;
                    }

                    try
                    {
                        foreach (var action in workflow.Actions.OrderBy(x => x.Order))
                        {
                            await _actionDispatcher.ExecuteAsync(action, workflowExecutionContext);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    // log later
                    continue;
                }
            }
        }
    }
}
