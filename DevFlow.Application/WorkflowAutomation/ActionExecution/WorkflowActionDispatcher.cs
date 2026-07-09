using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.ActionExecution
{
    public class WorkflowActionDispatcher : IActionDispatcher
    {
        private readonly IReadOnlyDictionary<WorkflowActionType, IActionExecutor> _executor;
        public WorkflowActionDispatcher(IEnumerable<IActionExecutor> executors)
        {
            _executor = executors.ToDictionary(x => x.Type);
        }
        public async Task ExecuteAsync(WorkflowAction action, WorkflowExecutionContext workflowExecutionContext)
        {
            if(!_executor.TryGetValue(action.ActionType,out var executor))
            {
                throw new InvalidOperationException(
            $"No executor registered for action '{action.ActionType}'.");
            }
            await executor.ExecuteAsync(action, workflowExecutionContext);
        }
    }
}
